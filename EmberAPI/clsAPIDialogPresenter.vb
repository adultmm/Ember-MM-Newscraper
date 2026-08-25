' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################
' ################################################################################
' # This file is part of Ember Media Manager.                                    #
' #                                                                              #
' # Ember Media Manager is free software: you can redistribute it and/or modify  #
' # it under the terms of the GNU General Public License as published by         #
' # the Free Software Foundation, either version 3 of the License, or            #
' # (at your option) any later version.                                          #
' #                                                                              #
' # Ember Media Manager is distributed in the hope that it will be useful,       #
' # but WITHOUT ANY WARRANTY; without even the implied warranty of               #
' # MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                #
' # GNU General Public License for more details.                                 #
' #                                                                              #
' # You should have received a copy of the GNU General Public License            #
' # along with Ember Media Manager.  If not, see <http://www.gnu.org/licenses/>. #
' ################################################################################

Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms
Imports NLog

''' <summary>
''' Shows scraper / search dialogs according to GeneralDialogsStayOnAppDesktop
''' and GeneralDialogsDoNotSwitchDesktop. Owned modeless display keeps the main
''' window clickable. Call from the thread that created the form.
''' </summary>
Public Class DialogPresenter

    Shared logger As Logger = LogManager.GetCurrentClassLogger()

    Private Shared ReadOnly _activeDialogs As New List(Of Form)
    Private Shared ReadOnly _activeDialogLock As New Object()

#Region "Constants"

    Private Const GWL_EXSTYLE As Integer = -20
    Private Const WS_EX_NOACTIVATE As Integer = &H8000000
    Private Const SW_SHOWNOACTIVATE As Integer = 4
    Private Const SWP_NOSIZE As UInteger = &H1
    Private Const SWP_NOMOVE As UInteger = &H2
    Private Const SWP_NOACTIVATE As UInteger = &H10
    Private Const SWP_SHOWWINDOW As UInteger = &H40
    Private Const LSFW_LOCK As UInteger = 1
    Private Const LSFW_UNLOCK As UInteger = 2
    Private Const VirtualDesktopManagerClsid As String = "aa509086-5ca9-4c25-8f95-589d3c07b48a"

#End Region 'Constants

#Region "Methods"

    Public Shared ReadOnly Property HasActiveDialog As Boolean
        Get
            SyncLock _activeDialogLock
                Return _activeDialogs.Count > 0
            End SyncLock
        End Get
    End Property

    Public Shared Function Present(dialog As Form) As DialogResult
        If dialog Is Nothing Then
            Throw New ArgumentNullException("dialog")
        End If

        If ScrapeCancellation.IsRequested Then
            logger.Trace("[DialogPresenter] [Present] [Abort] Scrape cancellation requested")
            Return DialogResult.Abort
        End If

        Dim stayOnAppDesktop As Boolean = True
        Dim doNotSwitchDesktop As Boolean = False
        If Master.eSettings IsNot Nothing Then
            stayOnAppDesktop = Master.eSettings.GeneralDialogsStayOnAppDesktop
            doNotSwitchDesktop = Master.eSettings.GeneralDialogsDoNotSwitchDesktop
        End If

        Dim uiForm As Form = FindUiForm(dialog)
        If uiForm IsNot Nothing AndAlso uiForm.InvokeRequired AndAlso dialog.InvokeRequired Then
            Return CType(uiForm.Invoke(New Func(Of DialogResult)(Function() PresentCore(dialog, stayOnAppDesktop, doNotSwitchDesktop))), DialogResult)
        End If

        Return PresentCore(dialog, stayOnAppDesktop, doNotSwitchDesktop)
    End Function

    Public Shared Function RunOnUIThread(Of T)(func As Func(Of T)) As T
        If func Is Nothing Then
            Throw New ArgumentNullException("func")
        End If
        Dim target As Form = FindUiForm(Nothing)
        If target IsNot Nothing AndAlso target.InvokeRequired Then
            Dim result As T = Nothing
            target.Invoke(New Action(Sub() result = func()))
            Return result
        End If
        Return func()
    End Function

    Public Shared Sub RunOnUIThread(action As Action)
        If action Is Nothing Then
            Throw New ArgumentNullException("action")
        End If
        RunOnUIThread(Of Object)(Function()
                                     action()
                                     Return Nothing
                                 End Function)
    End Sub

    ''' <summary>
    ''' Closes the dialog currently shown by <see cref="Present"/>, e.g. when the
    ''' main window Cancel Scraper button is pressed while a search dialog is open.
    ''' </summary>
    Public Shared Sub CancelActive()
        Dim snapshot As List(Of Form) = Nothing
        SyncLock _activeDialogLock
            If _activeDialogs.Count > 0 Then
                snapshot = New List(Of Form)(_activeDialogs)
            End If
        End SyncLock
        If snapshot Is Nothing Then
            Return
        End If

        For i As Integer = snapshot.Count - 1 To 0 Step -1
            Dim dialog As Form = snapshot(i)
            Try
                If dialog Is Nothing OrElse dialog.IsDisposed Then
                    Continue For
                End If

                Dim closeDialog As Action = Sub()
                                              If dialog.IsDisposed OrElse Not dialog.Visible Then
                                                  Return
                                              End If
                                              If dialog.DialogResult = DialogResult.None Then
                                                  dialog.DialogResult = DialogResult.Abort
                                              End If
                                              dialog.Close()
                                          End Sub

                If dialog.InvokeRequired Then
                    dialog.BeginInvoke(closeDialog)
                Else
                    closeDialog()
                End If
            Catch ex As Exception
                logger.Warn(ex, "[DialogPresenter] [CancelActive] Failed to close active dialog")
            End Try
        Next
    End Sub

    Private Shared Function PresentCore(dialog As Form, stayOnAppDesktop As Boolean, doNotSwitchDesktop As Boolean) As DialogResult
        Dim ownerHandle As IntPtr = IntPtr.Zero
        If stayOnAppDesktop Then
            dialog.ShowInTaskbar = False
            ownerHandle = GetOwnerHandle(dialog)
            If ownerHandle <> IntPtr.Zero Then
                MoveToOwnerDesktop(dialog, ownerHandle)
            End If
        End If

        If stayOnAppDesktop AndAlso doNotSwitchDesktop AndAlso ownerHandle <> IntPtr.Zero AndAlso Not IsOnCurrentDesktop(ownerHandle) Then
            ' Do not Show(owner): owned windows activate the owner and Windows
            ' switches to that virtual desktop.
            ShowWithoutActivating(dialog)
        ElseIf stayOnAppDesktop AndAlso ownerHandle <> IntPtr.Zero Then
            dialog.Show(New NativeWindowOwner(ownerHandle))
        Else
            dialog.Show()
        End If

        SyncLock _activeDialogLock
            _activeDialogs.Add(dialog)
        End SyncLock
        Try
            Return WaitUntilClosed(dialog)
        Finally
            SyncLock _activeDialogLock
                _activeDialogs.Remove(dialog)
            End SyncLock
        End Try
    End Function

    Private Shared Function WaitUntilClosed(dialog As Form) As DialogResult
        Dim closedResult As DialogResult = DialogResult.None
        AddHandler dialog.FormClosed, Sub(sender As Object, e As FormClosedEventArgs)
                                          closedResult = DirectCast(sender, Form).DialogResult
                                      End Sub

        While Not dialog.IsDisposed AndAlso dialog.Visible
            If ScrapeCancellation.IsRequested AndAlso dialog.DialogResult = DialogResult.None Then
                dialog.DialogResult = DialogResult.Abort
            End If
            If dialog.DialogResult <> DialogResult.None Then
                dialog.Close()
                Exit While
            End If
            Application.DoEvents()
            Thread.Sleep(10)
        End While

        If closedResult <> DialogResult.None Then
            Return closedResult
        End If
        If dialog.IsDisposed Then
            Return DialogResult.Cancel
        End If
        Return dialog.DialogResult
    End Function

    Private Shared Sub ShowWithoutActivating(dialog As Form)
        Dim previousForeground As IntPtr = GetForegroundWindow()
        Dim hwnd As IntPtr = dialog.Handle
        Dim previousExStyle As IntPtr = GetWindowLongPtr(hwnd, GWL_EXSTYLE)
        SetWindowLongPtr(hwnd, GWL_EXSTYLE, New IntPtr(previousExStyle.ToInt64() Or WS_EX_NOACTIVATE))
        LockSetForegroundWindow(LSFW_LOCK)
        Try
            ShowWindow(hwnd, SW_SHOWNOACTIVATE)
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE Or SWP_NOACTIVATE Or SWP_SHOWWINDOW)
            dialog.Show()
        Finally
            LockSetForegroundWindow(LSFW_UNLOCK)
            SetWindowLongPtr(hwnd, GWL_EXSTYLE, previousExStyle)
            RestoreForeground(previousForeground)
        End Try
    End Sub

    Private Shared Sub RestoreForeground(previousForeground As IntPtr)
        If previousForeground = IntPtr.Zero Then
            Return
        End If
        If GetForegroundWindow() <> previousForeground Then
            SetForegroundWindow(previousForeground)
        End If
    End Sub

    Private Shared Function IsOnCurrentDesktop(hwnd As IntPtr) As Boolean
        Try
            Dim manager As IVirtualDesktopManager = GetVirtualDesktopManager()
            If manager Is Nothing Then
                Return True
            End If
            Return manager.IsWindowOnCurrentVirtualDesktop(hwnd)
        Catch
            Return True
        End Try
    End Function

    Private Shared Function FindUiForm(exclude As Form) As Form
        Try
            If ModulesManager.Instance IsNot Nothing AndAlso ModulesManager.Instance.RuntimeObjects IsNot Nothing Then
                Dim runtime = ModulesManager.Instance.RuntimeObjects
                If runtime.MainForm IsNot Nothing AndAlso Not runtime.MainForm.IsDisposed AndAlso Not Object.ReferenceEquals(runtime.MainForm, exclude) Then
                    Return runtime.MainForm
                End If
                If runtime.MainTabControl IsNot Nothing Then
                    Dim fromTab As Form = runtime.MainTabControl.FindForm()
                    If fromTab IsNot Nothing AndAlso Not fromTab.IsDisposed AndAlso Not Object.ReferenceEquals(fromTab, exclude) Then
                        Return fromTab
                    End If
                End If
            End If
        Catch
        End Try

        Dim owner As Form = Nothing
        For Each f As Form In Application.OpenForms
            If f IsNot Nothing AndAlso Not f.IsDisposed AndAlso f.Visible AndAlso f.WindowState <> FormWindowState.Minimized AndAlso Not Object.ReferenceEquals(f, exclude) Then
                owner = f
            End If
        Next
        Return owner
    End Function

    Private Shared Function GetOwnerHandle(dialog As Form) As IntPtr
        Dim uiForm As Form = FindUiForm(dialog)
        If uiForm Is Nothing Then
            Return IntPtr.Zero
        End If
        Try
            If uiForm.InvokeRequired Then
                Return CType(uiForm.Invoke(New Func(Of IntPtr)(Function() If(uiForm.IsHandleCreated, uiForm.Handle, IntPtr.Zero))), IntPtr)
            End If
            If uiForm.IsHandleCreated Then
                Return uiForm.Handle
            End If
        Catch
        End Try
        Return IntPtr.Zero
    End Function

    Private Shared Sub MoveToOwnerDesktop(dialog As Form, ownerHandle As IntPtr)
        Try
            Dim manager As IVirtualDesktopManager = GetVirtualDesktopManager()
            If manager Is Nothing Then
                Return
            End If
            Dim unused = dialog.Handle
            Dim desktopId As Guid = manager.GetWindowDesktopId(ownerHandle)
            manager.MoveWindowToDesktop(dialog.Handle, desktopId)
        Catch
        End Try
    End Sub

    Private Shared Function GetVirtualDesktopManager() As IVirtualDesktopManager
        Try
            Dim managerType As Type = Type.GetTypeFromCLSID(New Guid(VirtualDesktopManagerClsid))
            If managerType Is Nothing Then
                Return Nothing
            End If
            Return CType(Activator.CreateInstance(managerType), IVirtualDesktopManager)
        Catch
            Return Nothing
        End Try
    End Function

#End Region 'Methods

#Region "Nested Types"

    Private Class NativeWindowOwner
        Implements IWin32Window

        Private ReadOnly _handle As IntPtr

        Public Sub New(handle As IntPtr)
            _handle = handle
        End Sub

        Public ReadOnly Property Handle As IntPtr Implements IWin32Window.Handle
            Get
                Return _handle
            End Get
        End Property
    End Class

    <ComImport>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    <Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")>
    Private Interface IVirtualDesktopManager
        Function IsWindowOnCurrentVirtualDesktop(topLevelWindow As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
        Function GetWindowDesktopId(topLevelWindow As IntPtr) As Guid
        Sub MoveWindowToDesktop(topLevelWindow As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> desktopId As Guid)
    End Interface

#End Region 'Nested Types

#Region "Native"

    <DllImport("user32.dll", EntryPoint:="GetWindowLongPtr")>
    Private Shared Function GetWindowLongPtr(hWnd As IntPtr, nIndex As Integer) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="SetWindowLongPtr")>
    Private Shared Function SetWindowLongPtr(hWnd As IntPtr, nIndex As Integer, dwNewLong As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function LockSetForegroundWindow(uLockCode As UInteger) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function ShowWindow(hWnd As IntPtr, nCmdShow As Integer) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SetWindowPos(hWnd As IntPtr, hWndInsertAfter As IntPtr, X As Integer, Y As Integer, cx As Integer, cy As Integer, uFlags As UInteger) As Boolean
    End Function

#End Region 'Native

End Class
