' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.Win32
Imports NLog

''' <summary>
''' Detects long-path access failures in EmberAPI (e.g. library scan) and forwards them to the UI layer.
''' </summary>
Public Module LongPathAccessNotify

#Region "Fields"

    Private ReadOnly logger As Logger = LogManager.GetCurrentClassLogger()
    Private Const Win32LongPathsRegPath As String = "SYSTEM\CurrentControlSet\Control\FileSystem"
    Private Const Win32LongPathsRegName As String = "LongPathsEnabled"
    Private Const MaxPath As Integer = 260
    Private Const HResultPathTooLong As Integer = &H800700CE
    Private Const NotifyDedupeSeconds As Double = 2.0

    Private ReadOnly _notifySync As New Object()
    Private _lastNotifyPathKey As String
    Private _lastNotifyUtc As DateTime = DateTime.MinValue

    ''' <summary>Set by EmberMediaManager at startup; invokes long-path UI offer on the UI thread.</summary>
    Public NotifyAction As Action(Of Exception, String)

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

    Public Function IsLongPathRelatedException(ByVal ex As Exception, Optional ByVal contextPath As String = Nothing) As Boolean
        Return Not String.IsNullOrEmpty(GetDetectionReason(ex, contextPath))
    End Function

    Public Sub NotifyIfLongPathRelated(ByVal ex As Exception, Optional ByVal contextPath As String = Nothing, Optional ByVal source As String = Nothing)
        Dim reason As String = GetDetectionReason(ex, contextPath)
        If String.IsNullOrEmpty(reason) Then Return
        If NotifyAction Is Nothing Then Return

        Dim effectivePath As String = ResolveEffectivePath(ex, contextPath)
        If IsDuplicateNotify(effectivePath) Then Return

        logger.Debug("[LongPath] Detected ({0}) source={1} ex={2} path={3}",
                     reason,
                     If(String.IsNullOrEmpty(source), "unknown", source),
                     If(ex Is Nothing, "none", ex.GetType().Name),
                     If(String.IsNullOrEmpty(effectivePath), "none", effectivePath))

        Try
            NotifyAction.Invoke(ex, effectivePath)
        Catch invokeEx As Exception
            logger.Debug(invokeEx, "[LongPath] NotifyAction failed")
        End Try
    End Sub

    ''' <summary>
    ''' When a deep path is missing but its parent exists, long-path limits may be blocking access.
    ''' </summary>
    Public Sub NotifyIfMissingPathMayBeLong(ByVal path As String, Optional ByVal source As String = Nothing)
        If String.IsNullOrEmpty(path) OrElse IsWin32LongPathsEnabled() Then Return
        If path.Length < MaxPath Then Return

        Dim parent As String = IO.Path.GetDirectoryName(path)
        If String.IsNullOrEmpty(parent) Then Return
        If Not Directory.Exists(parent) Then Return
        If Directory.Exists(path) Then Return

        NotifyIfLongPathRelated(Nothing, path, If(String.IsNullOrEmpty(source), "missing-path-heuristic", source))
    End Sub

    Private Function IsDuplicateNotify(ByVal effectivePath As String) As Boolean
        If String.IsNullOrEmpty(effectivePath) Then Return False

        Dim pathKey As String = effectivePath.ToLowerInvariant()
        SyncLock _notifySync
            If pathKey = _lastNotifyPathKey AndAlso (DateTime.UtcNow - _lastNotifyUtc).TotalSeconds < NotifyDedupeSeconds Then
                Return True
            End If
            _lastNotifyPathKey = pathKey
            _lastNotifyUtc = DateTime.UtcNow
            Return False
        End SyncLock
    End Function

    Private Function ResolveEffectivePath(ByVal ex As Exception, ByVal contextPath As String) As String
        If Not String.IsNullOrEmpty(contextPath) Then Return contextPath

        Dim current As Exception = ex
        While current IsNot Nothing
            Dim pathInMessage As String = ExtractPathFromExceptionMessage(current.Message)
            If Not String.IsNullOrEmpty(pathInMessage) Then Return pathInMessage
            current = current.InnerException
        End While

        Return Nothing
    End Function

    Private Function GetDetectionReason(ByVal ex As Exception, Optional ByVal contextPath As String = Nothing) As String
        If ex Is Nothing Then
            If IsLikelyLongPath(contextPath) Then Return "missing-path"
            Return Nothing
        End If

        Dim current As Exception = ex
        While current IsNot Nothing
            If TypeOf current Is PathTooLongException Then Return "PathTooLongException"
            If current.HResult = HResultPathTooLong Then Return "HRESULT_PATH_TOO_LONG"

            Dim message As String = current.Message
            If Not String.IsNullOrEmpty(message) AndAlso message.IndexOf("too long", StringComparison.OrdinalIgnoreCase) >= 0 Then
                Return "message-too-long"
            End If

            If TypeOf current Is DirectoryNotFoundException OrElse TypeOf current Is IOException Then
                Dim pathInMessage As String = ExtractPathFromExceptionMessage(message)
                If IsLikelyLongPath(pathInMessage) Then Return "io-with-long-path-in-message"
                ' Localized exception text may use quote chars we cannot pair; caller path is a reliable fallback.
                If IsLikelyLongPath(contextPath) Then Return "io-with-long-context-path"
            End If

            current = current.InnerException
        End While

        Return Nothing
    End Function

    Private Function IsLikelyLongPath(ByVal path As String) As Boolean
        Return Not String.IsNullOrEmpty(path) AndAlso path.Length >= MaxPath AndAlso Not IsWin32LongPathsEnabled()
    End Function

    Private Function ExtractPathFromExceptionMessage(ByVal message As String) As String
        If String.IsNullOrEmpty(message) Then Return Nothing

        ' localized .NET often wraps paths as „E:\..." (U+201E … U+201D)
        Dim lowNineQuote As Char = ChrW(&H201E)
        Dim highNineQuote As Char = ChrW(&H201D)
        Dim leftDoubleQuote As Char = ChrW(&H201C)
        Dim endPairIdx As Integer = message.LastIndexOf(highNineQuote)
        If endPairIdx > 0 Then
            Dim startPairIdx As Integer = message.LastIndexOf(lowNineQuote, endPairIdx - 1)
            If startPairIdx >= 0 Then
                Dim pairedCandidate As String = message.Substring(startPairIdx + 1, endPairIdx - startPairIdx - 1)
                If LooksLikeWindowsPath(pairedCandidate) Then Return pairedCandidate
            End If
            startPairIdx = message.LastIndexOf(leftDoubleQuote, endPairIdx - 1)
            If startPairIdx >= 0 Then
                Dim pairedCandidate As String = message.Substring(startPairIdx + 1, endPairIdx - startPairIdx - 1)
                If LooksLikeWindowsPath(pairedCandidate) Then Return pairedCandidate
            End If
        End If

        ' Same opening/closing quote characters (ASCII and Unicode)
        Dim quoteChars As Char() = {"'"c, """"c, lowNineQuote, leftDoubleQuote, highNineQuote}
        For Each quoteChar As Char In quoteChars
            Dim endQuote As Integer = message.LastIndexOf(quoteChar)
            If endQuote <= 0 Then Continue For
            Dim startQuote As Integer = message.LastIndexOf(quoteChar, endQuote - 1)
            If startQuote < 0 Then Continue For
            Dim candidate As String = message.Substring(startQuote + 1, endQuote - startQuote - 1)
            If LooksLikeWindowsPath(candidate) Then Return candidate
        Next

        ' Last resort: first drive-letter path embedded in the message
        Dim match As Match = Regex.Match(message, "([A-Za-z]:\\(?:[^\\r\\n""']+\\)*[^\\r\\n""']+)")
        If match.Success Then
            Dim regexCandidate As String = match.Groups(1).Value
            If LooksLikeWindowsPath(regexCandidate) Then Return regexCandidate
        End If

        Return Nothing
    End Function

    Private Function LooksLikeWindowsPath(ByVal path As String) As Boolean
        Return path.Length >= 3 AndAlso Char.IsLetter(path(0)) AndAlso path(1) = ":"c AndAlso path(2) = "\"c
    End Function

#End Region 'Methods

End Module
