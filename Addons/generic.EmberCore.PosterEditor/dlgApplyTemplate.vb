Imports System.Windows.Forms
Imports System.Data.SQLite
Imports EmberAPI
Imports System.Drawing
Imports generic.EmberCore.PosterEditor
Imports NLog
Imports System.IO

Public Class dlgApplyTemplate

    Shared logger As Logger = NLog.LogManager.GetCurrentClassLogger()

    Private _posterEditor As PosterEditor

    Private _overlayImage As Image
    Private _template As PosterTemplate

    Private _movieIds As List(Of Long)

    Private _cancelled As Boolean = False

    Private _readyCount As Integer = 0
    Private _skippedCount As Integer = 0
    Private _errorCount As Integer = 0

    Private Declare Function SendMessage Lib "user32.dll" Alias "SendMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As String) As Integer

    Public Sub New(posterEditor As PosterEditor)
        Me._posterEditor = posterEditor

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'FIMXE: i18n
        'radio buttons

        'FIMXE: i18n
        lblReady.Text = "Ready:"
        lblSkipped.Text = "Skipped:"
        lblError.Text = "Error:"
        RefreshCounters()

        pnlProgress.Location = pnlSettings.Location
        pnlProgress.Size = pnlSettings.Size
        pnlProgress.Visible = False

        'FIMXE: i18n
        'btnYes.Text = "Yes"
        'btnNo
        'btnCancel
        'btnOK

        btnCancel.Visible = False
        btnOK.Visible = False

    End Sub

    Private Sub btnYes_Click(sender As Object, e As EventArgs) Handles btnYes.Click
        btnYes.Visible = False
        btnNo.Visible = False

        btnCancel.Visible = True
        btnOK.Visible = False

        lblMessage.Visible = False

        pnlSettings.Visible = False
        pnlProgress.Visible = True

        If rbWithoutPoster.Checked Then
            'FIXME: 18n
            lblSkippedDetails.Text = "Because poster already exists."
        ElseIf rbUntouchedPoster.Checked Then
            'FIXME: 18n
            lblSkippedDetails.Text = "Because a template has been already applied."
        ElseIf rbUntouchedBaseImage.Checked Then
            'FIXME: 18n
            lblSkippedDetails.Text = "Because there is no untouched base image."
        ElseIf rbAll.Checked Then
            lblSkippedDetails.Visible = False
            lblSkipped.Visible = False
            lblSkippedValue.Visible = False
        Else
            Throw New InvalidOperationException("No radio button selected.")
        End If

        bwPosterGenerator.RunWorkerAsync()
    End Sub

    Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.No
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If bwPosterGenerator.IsBusy Then bwPosterGenerator.CancelAsync()
        While bwPosterGenerator.IsBusy
            Application.DoEvents()
            Threading.Thread.Sleep(50)
        End While

        btnCancel.Enabled = False
        btnOK.Visible = True
        btnOK.Enabled = True

        'we don't close the dialog automatically to keep time to read the counters 
        'Me.Close()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.DialogResult = If(_cancelled, DialogResult.Cancel, DialogResult.Yes)
        Me.Close()
    End Sub

    Friend Function ShowToContainigFolder(containingFolderPath As String, template As PosterTemplate, overlayImage As Image) As List(Of Long)
        _template = template
        _overlayImage = overlayImage

        Using SQLNewcommand As SQLite.SQLiteCommand = Master.DB.MyVideosDBConn.CreateCommand()
            SQLNewcommand.CommandText = "SELECT movie.idMovie AS movieId FROM movie 
                WHERE movie.MoviePath like :path;"

            If containingFolderPath.Chars(containingFolderPath.Length - 1) <> Path.DirectorySeparatorChar Then
                containingFolderPath += Path.DirectorySeparatorChar
            End If
            SQLNewcommand.Parameters.AddWithValue(":path", containingFolderPath & "%")

            _movieIds = New List(Of Long)
            Using SQLReader As SQLite.SQLiteDataReader = SQLNewcommand.ExecuteReader()
                While SQLReader.Read
                    _movieIds.Add(DirectCast(SQLReader("movieId"), Long))
                End While
            End Using
        End Using

        If _movieIds.Count > 0 Then
            'FIXME: i18n
            lblMessage.Text = String.Format("Do you want to apply the template and regenerate posters of {1} movies in {0} folder...", containingFolderPath, _movieIds.Count)
            _readyCount = 0
            _skippedCount = 0
            _errorCount = 0
            RefreshCounters()
        Else
            'FIXME: i18n
            lblMessage.Text = String.Format("There's no movie in {0} folder.", containingFolderPath)
            btnOK.Enabled = True
            btnOK.Visible = True
            btnYes.Visible = False
            btnNo.Visible = False
            btnCancel.Visible = False
        End If

        'FIXME: i18n
        Me.Text = String.Format("Regenerate posters for movies in {0} folder", containingFolderPath)

        Me.ShowDialog()

        Return _movieIds
    End Function

    Friend Function ShowForStudio(studio As String, template As PosterTemplate, overlayImage As Image) As List(Of Long)
        _template = template
        _overlayImage = overlayImage

        Using SQLNewcommand As SQLite.SQLiteCommand = Master.DB.MyVideosDBConn.CreateCommand()
            SQLNewcommand.CommandText = "SELECT movie.idMovie AS movieId FROM movie 
                INNER JOIN studiolinkmovie ON (studiolinkmovie.idMovie = movie.idMovie)
                INNER JOIN studio ON (studio.idStudio = studiolinkmovie.idStudio)
                WHERE studio.strStudio=:studio;"
            SQLNewcommand.Parameters.AddWithValue(":studio", studio)

            _movieIds = New List(Of Long)
            Using SQLReader As SQLite.SQLiteDataReader = SQLNewcommand.ExecuteReader()
                While SQLReader.Read
                    _movieIds.Add(DirectCast(SQLReader("movieId"), Long))
                End While
            End Using
        End Using

        If _movieIds.Count > 0 Then
            'FIXME: i18n
            lblMessage.Text = String.Format("Do you want to apply the template and regenerate posters of {1} movies with {0} studio...", studio, _movieIds.Count)
            _readyCount = 0
            _skippedCount = 0
            _errorCount = 0
            RefreshCounters()
        Else
            'FIXME: i18n
            lblMessage.Text = String.Format("There's no movie with {0} studio", studio)
            btnOK.Enabled = True
            btnOK.Visible = True
            btnYes.Visible = False
            btnNo.Visible = False
            btnCancel.Visible = False
        End If

        'FIXME: i18n
        Me.Text = String.Format("Regenerate posters for movies of {0} Studio", studio)

        Me.ShowDialog()

        Return _movieIds

    End Function

    Private Sub RefreshCounters()
        lblReadyValue.Text = _readyCount.ToString
        lblSkippedValue.Text = _skippedCount.ToString
        lblErrorValue.Text = _errorCount.ToString
    End Sub

    Private Sub bwPosterGenerator_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bwPosterGenerator.DoWork
        Dim i As Integer = 0
        bwPosterGenerator.ReportProgress(0)

        For Each id In _movieIds
            If bwPosterGenerator.CancellationPending Then
                e.Cancel = True
                Application.DoEvents()
                Return
            End If
            Dim movie As Database.DBElement = Nothing
            Try
                movie = Master.DB.Load_Movie(id)
                If rbWithoutPoster.Checked Then
                    If String.IsNullOrEmpty(movie.ImagesContainer.Poster.LocalFilePath) Then
                        GeneratePoster(movie)
                    Else
                        _skippedCount += 1
                    End If
                ElseIf rbUntouchedPoster.Checked Then
                    movie.ImagesContainer.Poster.LoadAndCache(Enums.ContentType.Movie, True, True)
                    If String.IsNullOrEmpty(movie.ImagesContainer.Poster.LocalFilePath) OrElse Not ExifHelper.HasExifInfo(movie.ImagesContainer.Poster.ImageOriginal.Image) Then
                        GeneratePoster(movie)
                    Else
                        _skippedCount += 1
                    End If
                ElseIf rbUntouchedBaseImage.Checked Then
                    If String.IsNullOrEmpty(movie.ImagesContainer.Poster.LocalFilePath) OrElse Not ExifHelper.HasExifInfo(movie.ImagesContainer.Poster.ImageOriginal.Image) _
                        OrElse PosterTemplateHelper.GetExistingUntouchedBaseFilePathForMovie(movie) IsNot Nothing Then
                        GeneratePoster(movie)
                    Else
                        _skippedCount += 1
                    End If
                ElseIf rbAll.Checked Then
                    GeneratePoster(movie)
                Else
                    Throw New InvalidOperationException("No radio button selected.")
                End If
            Catch ex As Exception
                _errorCount += 1
                logger.Error(ex, "Error during generate poster for movie {1}, id={0}.", id, If(movie IsNot Nothing, movie.ListTitle, ""))
            End Try

            Threading.Thread.Sleep(500)

            i += 1
            bwPosterGenerator.ReportProgress(CInt(i / _movieIds.Count * 100))
            Application.DoEvents()
        Next
        bwPosterGenerator.ReportProgress(100)
    End Sub

    Private Sub GeneratePoster(movie As Database.DBElement)
        Dim newPoster As Image = Nothing
        Try
            movie.ImagesContainer.Poster.LoadAndCache(Enums.ContentType.Movie, True, True)
            newPoster = PosterTemplateHelper.ApplyTemplateToPoster(movie, _template, _overlayImage, Nothing, True, True, Nothing)
            If newPoster IsNot Nothing Then

                _posterEditor.RaiseGenericEvent(Enums.ModuleEventType.BeforeEdit_Movie, New List(Of Object)(New Object() {movie.ID}))

                movie.ImagesContainer.Poster.ImageOriginal.UpdateMSfromImg(newPoster)
                Master.DB.Save_Movie(movie, False, True, True, True, False)

                _posterEditor.RaiseGenericEvent(Enums.ModuleEventType.AfterEdit_Movie, New List(Of Object)(New Object() {movie.ID}))

                _readyCount += 1
            Else
                _errorCount += 1
            End If
        Finally
            PosterTemplateHelper.SafeDispose(newPoster)
        End Try
    End Sub

    Private Sub bwPosterGenerator_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles bwPosterGenerator.ProgressChanged
        pbProgress.Value = e.ProgressPercentage
        RefreshCounters()
    End Sub

    Private Sub bwPosterGenerator_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwPosterGenerator.RunWorkerCompleted
        If e.Cancelled Then
            Me._cancelled = True
            SetProgressBarToErrorState()
        End If

        btnCancel.Visible = False
        btnOK.Visible = True
        btnOK.Enabled = True

    End Sub

    Private Sub SetProgressBarToErrorState()
        SendMessage(pbProgress.Handle,
            &H400 + 16, ' WM_USER + PBM_SETSTATE
            &H2, 'PBST_ERROR
            "0")

        'Using gr As Graphics = pbProgress.CreateGraphics()
        '    'FIXME: i18n
        '    Dim text = "Cancelled"
        '    Dim textSize As SizeF = gr.MeasureString(text, SystemFonts.DefaultFont)
        '    gr.DrawString(text, SystemFonts.DefaultFont, Brushes.White, New PointF(CSng(pbProgress.Width / 2 - (textSize.Width / 2)), CSng(pbProgress.Height / 2 - (textSize.Height / 2))))
        'End Using

    End Sub
End Class
