' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports EmberAPI

Public Class dlgPartialDownloadSidecarClean

#Region "Nested Types"

    Public Enum SidecarChoice
        DeleteFiles
        KeepFiles
        Abort
    End Enum

#End Region 'Nested Types

#Region "Fields"

    Private _Choice As SidecarChoice = SidecarChoice.Abort

#End Region 'Fields

#Region "Properties"

    Public ReadOnly Property Choice As SidecarChoice
        Get
            Return _Choice
        End Get
    End Property

    Public ReadOnly Property ApplyToAllRemaining As Boolean
        Get
            Return chkApplyToAll.Checked
        End Get
    End Property

#End Region 'Properties

#Region "Methods"

    Public Sub New()
        InitializeComponent()
        Left = Master.AppPos.Left + (Master.AppPos.Width - Width) \ 2
        Top = Master.AppPos.Top + (Master.AppPos.Height - Height) \ 2
        StartPosition = FormStartPosition.Manual
    End Sub

    Public Overloads Function ShowDialog(ByVal displayTitle As String, ByVal filePath As String, ByVal sidecarPaths As IEnumerable(Of String)) As SidecarChoice
        SetUp(displayTitle, filePath, sidecarPaths)
        MyBase.ShowDialog()
        Return _Choice
    End Function

    Private Sub SetUp(ByVal displayTitle As String, ByVal filePath As String, ByVal sidecarPaths As IEnumerable(Of String))
        Text = "Partial download sidecar files" 'FIXME: i18n
        lblTitle.Text = displayTitle
        lblPath.Text = filePath
        lstSidecars.Items.Clear()
        If sidecarPaths IsNot Nothing Then
            For Each sidecarPath As String In sidecarPaths
                lstSidecars.Items.Add(sidecarPath)
            Next
        End If
        chkApplyToAll.Checked = False
        chkApplyToAll.Text = "Apply this choice to all remaining entries in this clean" 'FIXME: i18n
        btnDeleteFiles.Text = "Delete files" 'FIXME: i18n
        btnKeepFiles.Text = "Keep files" 'FIXME: i18n
        btnAbort.Text = "Abort" 'FIXME: i18n
    End Sub

    Private Sub btnDeleteFiles_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDeleteFiles.Click
        _Choice = SidecarChoice.DeleteFiles
        DialogResult = DialogResult.OK
    End Sub

    Private Sub btnKeepFiles_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnKeepFiles.Click
        _Choice = SidecarChoice.KeepFiles
        DialogResult = DialogResult.OK
    End Sub

    Private Sub btnAbort_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAbort.Click
        _Choice = SidecarChoice.Abort
        DialogResult = DialogResult.Cancel
    End Sub

#End Region 'Methods

End Class
