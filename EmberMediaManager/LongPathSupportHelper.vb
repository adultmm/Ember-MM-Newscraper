' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports System.Diagnostics
Imports System.IO
Imports System.Runtime.ExceptionServices
Imports System.Threading
Imports System.Windows.Forms
Imports EmberAPI
Imports Microsoft.VisualBasic.Constants
Imports Microsoft.Win32
Imports NLog

Public Module LongPathSupportHelper

#Region "Fields"

    Private ReadOnly logger As Logger = LogManager.GetCurrentClassLogger()
    Private Const Win32LongPathsRegPath As String = "SYSTEM\CurrentControlSet\Control\FileSystem"
    Private Const Win32LongPathsRegName As String = "LongPathsEnabled"

    'FIXME: i18n
    Private Const SettingsLocationHint As String = "You can also enable this later under Settings → General → Misc."
    'FIXME: i18n
    Private Const AskSettingsHint As String = "Use ""Ask about Windows long path support"" there to control these reminders."

    Private ReadOnly _sync As New Object()
    Private _firstChanceRegistered As Boolean
    Private _uiOfferQueued As Boolean
    Private _uiControl As Control

#End Region 'Fields

#Region "Methods"

    Public Function IsWin32LongPathsEnabled() As Boolean
        Try
            Using key = Registry.LocalMachine.OpenSubKey(Win32LongPathsRegPath, writable:=False)
                If key Is Nothing Then Return False
                Return Convert.ToInt32(key.GetValue(Win32LongPathsRegName, 0)) = 1
            End Using
        Catch ex As Exception
            logger.Debug(ex, "[LongPath] Failed to read LongPathsEnabled")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Enables HKLM LongPathsEnabled=1 (direct write or UAC-elevated reg.exe).
    ''' </summary>
    Public Function TryEnableWin32LongPaths() As Boolean
        Try
            Using key = Registry.LocalMachine.OpenSubKey(Win32LongPathsRegPath, writable:=True)
                If key IsNot Nothing Then
                    key.SetValue(Win32LongPathsRegName, 1, RegistryValueKind.DWord)
                    If IsWin32LongPathsEnabled() Then Return True
                End If
            End Using
        Catch ex As Exception
            logger.Debug(ex, "[LongPath] Direct registry write failed; will try elevated reg.exe")
        End Try

        If Not Environment.UserInteractive Then Return False

        Try
            Dim psi As New ProcessStartInfo With {
                .FileName = "reg.exe",
                .Arguments = "add ""HKLM\SYSTEM\CurrentControlSet\Control\FileSystem"" /v LongPathsEnabled /t REG_DWORD /d 1 /f",
                .Verb = "runas",
                .UseShellExecute = True,
                .CreateNoWindow = True,
                .WindowStyle = ProcessWindowStyle.Hidden
            }
            Dim p = Process.Start(psi)
            If p Is Nothing Then
                logger.Debug("[LongPath] Elevated reg.exe Process.Start returned Nothing")
                Return False
            End If
            p.WaitForExit()
            Return p.ExitCode = 0 AndAlso IsWin32LongPathsEnabled()
        Catch ex As Exception
            ' UAC cancel is common (Win32Exception); other Start/Wait failures also land here
            logger.Debug(ex, "[LongPath] Elevated reg.exe failed or UAC was cancelled")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Startup Yes/No/Cancel prompt when OS long paths are off and Ask is enabled.
    ''' Returns True if restart was initiated (caller should abort startup).
    ''' </summary>
    Public Function TryPromptAtStartup() As Boolean
        If IsWin32LongPathsEnabled() Then Return False
        If Not Master.eSettings.GeneralAskLongPathPrompt Then Return False
        If Not Environment.UserInteractive Then Return False
        If Master.isCL Then Return False
        ' Ember CLI flag: hide splash / run without main window UI (see clsAPICommandLine)
        If Master.appArgs IsNot Nothing AndAlso Master.appArgs.CommandLine.Contains("-nowindow") Then Return False

        'FIXME: i18n
        Dim message As String = String.Concat(
            "Windows long path support is disabled on this system.", vbCrLf, vbCrLf,
            "Without it, media files and folders whose full path exceeds 260 characters may fail during library scans and other file operations.", vbCrLf, vbCrLf,
            "Enabling requires administrator approval (UAC). The application will restart afterward so the change can take effect.", vbCrLf, vbCrLf,
            SettingsLocationHint, vbCrLf,
            AskSettingsHint)

        'FIXME: i18n
        Select Case ThreeButtonPromptHelper.Show(
            "Long Path Support",
            message,
            "Enable now",
            "Not now",
            "Don't ask again")
            Case ThreeButtonPromptHelper.ThreeButtonChoice.First
                If TryEnableWin32LongPaths() Then
                    'FIXME: i18n
                    MessageBox.Show(
                        "Windows long path support has been enabled. The application will restart so the change can take effect.",
                        "Long Path Support",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)
                    Application.Restart()
                    Return True
                Else
                    'FIXME: i18n
                    MessageBox.Show(
                        "Windows long path support could not be enabled. Approve the UAC prompt, or set HKLM\SYSTEM\CurrentControlSet\Control\FileSystem\LongPathsEnabled to 1, then restart.",
                        "Long Path Support",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
                    Return False
                End If
            Case ThreeButtonPromptHelper.ThreeButtonChoice.Third
                Master.eSettings.GeneralAskLongPathPrompt = False
                Master.eSettings.Save()
                Return False
            Case Else
                Return False
        End Select
    End Function

    Public Sub RegisterFirstChanceHandler(Optional ByVal uiControl As Control = Nothing)
        ' Always refresh the UI marshal target (even if the handler is already registered).
        If uiControl IsNot Nothing Then _uiControl = uiControl
        If _firstChanceRegistered Then Return
        AddHandler AppDomain.CurrentDomain.FirstChanceException, AddressOf OnFirstChanceException
        _firstChanceRegistered = True
    End Sub

    ''' <summary>
    ''' Offer enable/info UI after a long-path access failure (FirstChance, scanner, or Unhandled).
    ''' Concurrent offers are coalesced while a prompt is open; Continue allows a later retry.
    ''' </summary>
    Public Sub OfferPathTooLongResolution()
        QueueOfferOnUi()
    End Sub

    Private Sub OnFirstChanceException(ByVal sender As Object, ByVal e As FirstChanceExceptionEventArgs)
        ' Ask already gates the UI; exit here so FirstChance does not queue work when opted out.
        If Master.eSettings Is Nothing OrElse Not Master.eSettings.GeneralAskLongPathPrompt Then Return
        If Not Environment.UserInteractive Then Return
        If Master.isCL Then Return
        ' Ember CLI flag: hide splash / run without main window UI (see clsAPICommandLine)
        If Master.appArgs IsNot Nothing AndAlso Master.appArgs.CommandLine.Contains("-nowindow") Then Return

        SyncLock _sync
            If _uiOfferQueued Then Return
        End SyncLock

        ' Strict type/HRESULT only — broad heuristics belong on explicit scanner hooks with a context path.
        If Not LongPathAccessNotify.IsStrictLongPathRelatedException(e.Exception) Then Return

        LongPathAccessNotify.NotifyIfLongPathRelated(e.Exception, Nothing, "FirstChance")
    End Sub

    Private Sub QueueOfferOnUi()
        SyncLock _sync
            If _uiOfferQueued Then Return
            _uiOfferQueued = True
        End SyncLock

        Dim showOffer As Action = AddressOf ShowPathTooLongOfferOnUi

        Try
            Dim ctrl = _uiControl
            If ctrl IsNot Nothing AndAlso Not ctrl.IsDisposed AndAlso ctrl.IsHandleCreated Then
                ctrl.BeginInvoke(showOffer)
                Return
            End If

            If Application.OpenForms.Count > 0 Then
                Dim form = Application.OpenForms(0)
                If form IsNot Nothing AndAlso Not form.IsDisposed AndAlso form.IsHandleCreated Then
                    form.BeginInvoke(showOffer)
                    Return
                End If
            End If

            Dim sc = SynchronizationContext.Current
            If sc IsNot Nothing Then
                sc.Post(Sub(state) showOffer(), Nothing)
                Return
            End If
        Catch ex As Exception
            logger.Debug(ex, "[LongPath] Failed to queue PathTooLong UI offer")
        End Try

        ' No UI sync context yet — clear queue flag so a later offer can retry
        SyncLock _sync
            _uiOfferQueued = False
        End SyncLock
        logger.Debug("[LongPath] PathTooLong UI offer not queued yet (no UI sync context); will retry later")
    End Sub

    Private Sub ShowPathTooLongOfferOnUi()
        Try
            If IsWin32LongPathsEnabled() Then
                ShowAlreadyEnabledInfo()
            Else
                ShowEnablePromptFromRuntime()
            End If
        Catch ex As Exception
            logger.Warn(ex, "[LongPath] PathTooLong UI offer failed")
        Finally
            SyncLock _sync
                _uiOfferQueued = False
            End SyncLock
        End Try
    End Sub

    Private Sub ShowAlreadyEnabledInfo()
        ' Same Ask flag as enable prompts: Don't ask again / unchecked setting suppresses this too.
        If Not Master.eSettings.GeneralAskLongPathPrompt Then Return

        'FIXME: i18n
        Dim message As String = String.Concat(
            "A path that is too long was encountered, but Windows long path support is already enabled.", vbCrLf, vbCrLf,
            "This can still happen when a single path segment exceeds 255 characters, when a network/share or native API does not support long paths, or when a component bypasses the long-path-aware Win32 APIs.", vbCrLf, vbCrLf,
            "Check the application log for the affected path.")

        'FIXME: i18n
        MessageBox.Show(message, "Long Path Support", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub ShowEnablePromptFromRuntime()
        If Not Master.eSettings.GeneralAskLongPathPrompt Then Return

        'FIXME: i18n
        Dim message As String = String.Concat(
            "A path that exceeds the Windows MAX_PATH limit (260 characters) was encountered.", vbCrLf, vbCrLf,
            "Enabling Windows long path support usually resolves this for local media libraries.", vbCrLf, vbCrLf,
            "Enabling requires administrator approval (UAC). The application will restart afterward; any running scan or scrape will be interrupted.", vbCrLf, vbCrLf,
            SettingsLocationHint, vbCrLf,
            AskSettingsHint)

        'FIXME: i18n
        Select Case ThreeButtonPromptHelper.Show(
            "Long Path Support",
            message,
            "Enable now and restart",
            "Continue",
            "Don't ask again")
            Case ThreeButtonPromptHelper.ThreeButtonChoice.First
                If TryEnableWin32LongPaths() Then
                    'FIXME: i18n
                    MessageBox.Show(
                        "Windows long path support has been enabled. The application will restart so the change can take effect.",
                        "Long Path Support",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)
                    Application.Restart()
                Else
                    'FIXME: i18n
                    MessageBox.Show(
                        "Windows long path support could not be enabled. Approve the UAC prompt, or set HKLM\SYSTEM\CurrentControlSet\Control\FileSystem\LongPathsEnabled to 1, then restart.",
                        "Long Path Support",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
                End If
            Case ThreeButtonPromptHelper.ThreeButtonChoice.Third
                Master.eSettings.GeneralAskLongPathPrompt = False
                Master.eSettings.Save()
        End Select
    End Sub

#End Region 'Methods

End Module
