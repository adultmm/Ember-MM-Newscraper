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

Imports System.Windows.Forms
Imports System.IO
Imports EmberAPI
Imports NLog
Imports System.Drawing
Imports System.Runtime.Serialization
Imports System.Reflection
Imports generic.EmberCore.PosterEditor

Public Class frmMoviePosterEditor
    Implements Interfaces.MovieEditPanel, Interfaces.MovieEditPanel.OnShowListener

#Region "Fields"

    Shared logger As Logger = NLog.LogManager.GetCurrentClassLogger()

    Private _tmpDBElement As Database.DBElement
    Private _template As PosterTemplate


    Private _overlayImage As Image = Nothing
    Private _overlayImagePath As String = Nothing

    Private _posterOffset As Point = New Point(0, 0)
    Private _untouchedPoster As Image

    Private _drawSettings As PosterTemplateHelper.DrawSettings

    Private _drawSuspended As Integer
    Private _cmsSaveTemplate_Inited As Boolean
    Private _cmsApplyTemplate_Inited As Boolean

    Private _templatePath As String
    Private _posterEditor As PosterEditor

#End Region 'Fields


#Region "Properties"
    Public ReadOnly Property EditPanel As Panel Implements Interfaces.MovieEditPanel.EditPanel
        Get
            Return Me.pnlMain
        End Get
    End Property

    Public ReadOnly Property TabText As String Implements Interfaces.MovieEditPanel.TabText
        Get
            'FIXME: internationalization 
            Return "Poster Editor"
        End Get
    End Property

#End Region 'Properties


#Region "Methods"
    Public Sub New(posterEditor As PosterEditor)
        Me._posterEditor = posterEditor

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.Left = Master.AppPos.Left + (Master.AppPos.Width - Me.Width) \ 2
        Me.Top = Master.AppPos.Top + (Master.AppPos.Height - Me.Height) \ 2
        Me.StartPosition = FormStartPosition.Manual

        'Make pbPoster selectable
        Try
            pbPoster.TabStop = True
            Dim method As MethodInfo = pbPoster.GetType().GetMethod("SetStyle", BindingFlags.NonPublic Or BindingFlags.Instance)
            method.Invoke(pbPoster, {ControlStyles.Selectable, True})
        Catch ex As Exception
            logger.Warn("Cannot call SetStyle on PictureBox.")
        End Try

        'FIXME: i18n 
        tpGeneral.Text = "General"
        tpBaseImage.Text = "Base image"
        lblOverlayWidth.Text = "Width"

        'FIXME: i18n 
        btnOpenTemplate.Text = "Open Template"
        btnSaveTemplate.Text = "Save Template"
        btnApplyTemplate.Text = "Apply Template"

        'FIXME: i18n 
        cbExtractFrame.Text = "Extract frame if no poster"
        cbExtractFrame.Enabled = False
        cbExtractFrame.Checked = True

        'FIXME: i18n 
        lblFramePosition.Text = "Frame position"
        tbFrameExtractPercent.Enabled = False
        tbFrameExtractPercent.Value = 50
        txtFrameExtractPercent.Enabled = False
        txtFrameExtractPercent.Text = "50"

        'FIXME: i18n 
        lblBaseImageSize.Text = "Size"
        tbBaseImageSize.Enabled = False
        tbBaseImageSize.Value = 100
        txtBaseImageSize.Enabled = False
        txtBaseImageSize.Text = "100"

        'FIXME: i18n 
        lblBaseImageRotate.Text = "Rotate"
        tbBaseImageRotate.Enabled = False
        tbBaseImageRotate.Value = 0
        txtBaseImageRotate.Enabled = False
        txtBaseImageRotate.Text = "0"


        'FIXME: i18n 
        ttpForm.SetToolTip(btnUp, "Move the base image to up, use Shift for speed up.")
        ttpForm.SetToolTip(btnRight, "Move the base image to right, use Shift for speed up.")
        ttpForm.SetToolTip(btnDown, "Move the base image to down, use Shift for speed up.")
        ttpForm.SetToolTip(btnLeft, "Move the base image to left, use Shift for speed up.")
        ttpForm.SetToolTip(btnRestorePosition, "Restore the position of the base image.")
        ttpForm.SetToolTip(pbPoster, "Click here and use the arrow buttons to move the base image.")

    End Sub

    Public Sub Init(ByVal dbElement As Database.DBElement)
        'TODO: which parts of this method sould be in the constructor?

        Me._tmpDBElement = dbElement
        'some event handlers write it, so it's necessary
        Me._template = New PosterTemplate()
        Try
            SuspendDrawing()

            pbPoster.AllowDrop = True

            DrawPoster()

            'TODO: make these configurable
            _drawSettings = New PosterTemplateHelper.DrawSettings()
            _drawSettings.CropBorderPen = Pens.Red
            _drawSettings.BorderSize = 1

            cbCropEnabled.Text = Master.eLang.GetString(774, "Enabled")

            'FIXME: internationalization 
            'lblAspectRatio.Text = Master.eLang.GetString(, "Aspect ratio:")

            'FIXME: read aspect ratios from config
            Dim ratios = New List(Of PosterTemplate.AspectRatio)
            ratios.Add(New PosterTemplate.AspectRatio(1.5, "Default 2:3"))
            ratios.Add(New PosterTemplate.AspectRatio(1, "Square 1:1"))
            ratios.Add(New PosterTemplate.AspectRatio(184 / 130, "DVD Cover 184:130"))
            ratios.Add(New PosterTemplate.AspectRatio(148 / 128.5, "Blu-Ray Cover 148:128.5"))
            ratios.Add(New PosterTemplate.AspectRatio(1.5, "PLEX poster 2:3"))

            cbAspectRatio.DataSource = ratios
            cbAspectRatio.DisplayMember = "RatioLabel"
            cbAspectRatio.ValueMember = "Ratio"
            'cbAspectRatio.SelectedIndex = 0

            'FIXME: internationalization 
            'cbShowCropBorder.Text = Master.eLang.GetString(, "Show Crop Border")
            cbShowCropBorder.Checked = True
            'cbPreviewCropping.Text = Master.eLang.GetString(, "Preview Cropping")
            cbPreviewCropping.Checked = False

            cbOverlayEnabled.Text = Master.eLang.GetString(774, "Enabled")

            Dim templatewp = PosterTemplateHelper.DetectTemplate(_tmpDBElement, True)

            If templatewp Is Nothing AndAlso PosterEditor._showTestTemplate Then
                templatewp = PosterTemplateHelper.LoadTestTemplate()
            End If

            If templatewp IsNot Nothing Then
                Dim generatedBaseImage = New ImageResult()
                Dim newPoster = PosterTemplateHelper.ApplyTemplateToPoster(_tmpDBElement, templatewp, False, False, generatedBaseImage)
                Dim newPosterPath As String = Nothing
                Try
                    If PosterEditor._autoGeneratePoster Then
                        'template applied, and autogenerate enabled => poster changed
                        newPosterPath = PosterTemplateHelper.SavePosterToTemp(newPoster)
                        RaisePosterChangedEvent(newPosterPath)
                    End If

                    If generatedBaseImage.image IsNot Nothing Then
                        _untouchedPoster = generatedBaseImage.image
                    Else
                        _tmpDBElement.ImagesContainer.Poster.LoadAndCache(Enums.ContentType.Movie, True, True)
                        If _tmpDBElement.ImagesContainer.Poster.ImageOriginal.Image IsNot Nothing Then
                            _untouchedPoster = _tmpDBElement.ImagesContainer.Poster.ImageOriginal.Image
                        End If
                    End If

                    If Not LoadTemplate(templatewp, True) Then
                        logger.Error("Template applied but cannot be load: {0}", templatewp.Path)
                    End If
                Finally
                    PosterTemplateHelper.SafeDispose(newPoster)
                    PosterTemplateHelper.SafeDelete(newPosterPath)
                End Try
            Else
                _template = New PosterTemplate()
                _template.PosterRatio = New PosterTemplate.AspectRatio(1.5, "Default 2:3")
                _template.BackgroundColor = Color.Black
                _template.CropEnabled = True
                _template.Image.HorizontalAlignment = PosterTemplate.HorizontalAlignment.Center
                _template.Image.VerticalAlignment = PosterTemplate.VerticalAlignment.Middle
                _template.Image.WidthPercent = 50

                _tmpDBElement.ImagesContainer.Poster.LoadAndCache(Enums.ContentType.Movie, True, True)
                If _tmpDBElement.ImagesContainer.Poster.ImageOriginal.Image IsNot Nothing Then
                    _untouchedPoster = _tmpDBElement.ImagesContainer.Poster.ImageOriginal.Image
                Else
                    _untouchedPoster = PosterTemplateHelper.GenerateBaseImageByTemplate(_template, _tmpDBElement)
                End If

                LoadTemplateWithoutImage(_template)
            End If

            btnSaveAsPoster.Enabled = _untouchedPoster IsNot Nothing
            btnUp.Enabled = _untouchedPoster IsNot Nothing
            btnDown.Enabled = _untouchedPoster IsNot Nothing
            btnRight.Enabled = _untouchedPoster IsNot Nothing
            btnLeft.Enabled = _untouchedPoster IsNot Nothing
            btnRestorePosition.Enabled = _untouchedPoster IsNot Nothing

            btnReloadUntouched.Enabled = PosterTemplateHelper.GetExistingUntouchedBaseFilePathForMovie(_tmpDBElement) IsNot Nothing

        Catch ex As Exception
            logger.Error(ex)
        Finally
            ResumeDrawing()
        End Try

    End Sub

    Public Sub OnShow(DBElement As Database.DBElement) Implements Interfaces.MovieEditPanel.OnShowListener.OnShow
        _tmpDBElement = DBElement
        DrawPoster()
    End Sub

    Friend Sub OnPosterChangedDuringEdit(dbElement As Database.DBElement)
        Try
            SuspendDrawing()

            _tmpDBElement = dbElement

            If PosterEditor._autoGeneratePoster Then
                Dim newPoster As Image = Nothing
                Try
                    newPoster = PosterTemplateHelper.ApplyTemplateToPoster(dbElement, _template, _overlayImage, Nothing, False, False)
                    If newPoster IsNot Nothing Then
                        Dim newPosterPath = PosterTemplateHelper.SavePosterToTemp(newPoster)
                        'template applied => poster changed
                        RaisePosterChangedEvent(newPosterPath)
                    End If
                Finally
                    PosterTemplateHelper.SafeDispose(newPoster)
                End Try
            End If

            _tmpDBElement.ImagesContainer.Poster.LoadAndCache(Enums.ContentType.Movie, True, True)
            If _tmpDBElement.ImagesContainer.Poster.ImageOriginal.Image IsNot Nothing AndAlso Not ExifHelper.HasExifInfo(_tmpDBElement.ImagesContainer.Poster.ImageOriginal.Image) Then
                _untouchedPoster = _tmpDBElement.ImagesContainer.Poster.ImageOriginal.Image
            End If

            'new image => restore the offset (it'd be very confusing when autoGenerate mode is active, because applying the template don't use this offset)
            HandleRestorePosition()

            DrawPoster()
        Catch ex As Exception
            logger.Error(ex)
        Finally
            ResumeDrawing()
        End Try
    End Sub

    Private Function SetOverlayImageFromTemplate(twp As PosterTemplateWithPath, Optional silent As Boolean = False) As Boolean
        Return SetOverlayImage(Path.Combine(Path.GetDirectoryName(twp.Path), twp.Template.Image.Filename), silent)
    End Function

    Private Function SetOverlayImage(imagePath As String, silent As Boolean) As Boolean
        Try
            SuspendDrawing()
            If imagePath Is Nothing Then
                If _overlayImage IsNot Nothing Then
                    _overlayImage.Dispose()
                    _overlayImage = Nothing
                End If
                Me._overlayImagePath = Nothing
                cbOverlayEnabled.Checked = False
            Else
                Dim img = PosterTemplateHelper.LoadImageFromFile(imagePath)
                PosterTemplateHelper.SafeDispose(_overlayImage)
                _overlayImage = img
                Me._overlayImagePath = imagePath
                cbOverlayEnabled.Checked = True
            End If
            DrawPoster()
            Return True
        Catch ex As Exception
            Dim userMessage As String
            If TypeOf ex Is FileNotFoundException Then
                logger.Warn(ex, "Overlay image not found: {0}", imagePath)
                'FIXME: internationalization
                userMessage = String.Format("Overlay image not found: {0}", imagePath)
            Else
                logger.Warn(ex, "Cannot load overlay image: {0}, reason: {1}", imagePath, ex.Message)
                'FIXME: internationalization 
                userMessage = String.Format("Cannot load overlay image: {0}, reason: {1}", imagePath, ex.Message)
            End If
            If Not silent Then
                MsgBox(userMessage)
            End If
            Return False
        Finally
            ResumeDrawing()
        End Try
    End Function

    Private Function LoadTemplate(twp As PosterTemplateWithPath, Optional silent As Boolean = False) As Boolean
        Try
            SuspendDrawing()
            LoadTemplateWithoutImage(twp.Template)
            If twp.Template.Image.Enabled Then
                If String.IsNullOrWhiteSpace(twp.Template.Image.Filename) Then
                    SetOverlayImage(Nothing, True)
                    twp.Template.Image.Enabled = False
                Else
                    If Not SetOverlayImageFromTemplate(twp, silent) Then
                        twp.Template.Image.Enabled = False
                    End If
                End If
            Else
                SetOverlayImage(Nothing, True)
            End If
            cbOverlayEnabled.Checked = twp.Template.Image.Enabled
            If twp.Template.Image.Enabled Then
                If twp.Template.Image.WidthPercent > 0 Then
                    SetOverlayWidthText(twp.Template.Image.WidthPercent)
                Else
                    'FIXME: better option?
                    SetOverlayWidthText(50)
                End If
            End If
            _template = twp.Template
            _templatePath = twp.Path
            DrawPoster()
            Return True
        Catch ex As Exception
            logger.Warn("Cannot load template: {0}, reason: {1}", _templatePath, ex.Message)
            If Not silent Then
                'FIXME: internationalization 
                MsgBox(String.Format("Cannot load template: {0}, reason: {1}", _templatePath, ex.Message))
            End If
            Return False
        Finally
            ResumeDrawing()
        End Try
    End Function

    Private Sub SetOverlayWidthText(value As Double)
        txtOverlayWidth.Text = Strings.Format(value, "###0.00")
    End Sub

    Private Sub LoadTemplateWithoutImage(template As PosterTemplate)
        Try
            SuspendDrawing()
            cbCropEnabled.Checked = template.CropEnabled

            Dim ratios = DirectCast(cbAspectRatio.DataSource, List(Of PosterTemplate.AspectRatio))
            Dim ratioIndex = ratios.IndexOf(template.PosterRatio)
            If (ratioIndex < 0) Then
                ratioIndex = ratios.Count
                ratios.Add(template.PosterRatio)
            End If
            cbAspectRatio.SelectedIndex = ratioIndex

            Select Case template.Image.HorizontalAlignment
                Case PosterTemplate.HorizontalAlignment.Left
                    rbAlignLeft.Checked = True
                Case PosterTemplate.HorizontalAlignment.Center
                    rbAlignCenter.Checked = True
                Case PosterTemplate.HorizontalAlignment.Right
                    rbAlignRight.Checked = True
                Case PosterTemplate.HorizontalAlignment.Stretch
                    rbAlignStretch.Checked = True
            End Select
            Select Case template.Image.VerticalAlignment
                Case PosterTemplate.VerticalAlignment.Top
                    rbVAlignTop.Checked = True
                Case PosterTemplate.VerticalAlignment.Middle
                    rbVAlignMiddle.Checked = True
                Case PosterTemplate.VerticalAlignment.Bottom
                    rbVAlignBottom.Checked = True
                Case PosterTemplate.VerticalAlignment.Stretch
                    rbVAlignStretch.Checked = True
            End Select

            If template.Image.WidthPercent > 0 Then
                SetOverlayWidthText(template.Image.WidthPercent)
            End If

            Me._template = template
            DrawPoster()
        Finally
            ResumeDrawing()
        End Try
    End Sub

    Protected Sub SuspendDrawing()
        _drawSuspended += 1
    End Sub

    Protected Sub ResumeDrawing()
        _drawSuspended -= 1
        If _drawSuspended <= 0 Then
            If _drawSuspended < 0 Then
                logger.Error("SuspendDrawing() and ResumeDrawing() calls are unsyched.")
                _drawSuspended = 0
            End If
            DrawPoster()
        End If
    End Sub

    Private Sub DrawPoster()
        If _drawSuspended > 0 Then
            Return
        End If

        If _untouchedPoster IsNot Nothing Then

            Dim imageToDraw As Image
            If pbPoster.Image Is Nothing Then
                pbPoster.Image = New Bitmap(pbPoster.Width, pbPoster.Height)
            End If
            imageToDraw = pbPoster.Image

            'FIXME: !!! handle the case when no _untouchedPoster (like ApplyTemplateToPoster method)
            Dim cropRectangle = PosterTemplateHelper.DrawPosterForEdit(imageToDraw, _untouchedPoster, _template, _overlayImage, _posterOffset, _drawSettings)

            If _template.CropEnabled Then
                lblPosterSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), cropRectangle.Width, cropRectangle.Height)
                lblPosterSize.Visible = True
            Else
                lblPosterSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), _untouchedPoster.Width, _untouchedPoster.Height)
                lblPosterSize.Visible = True
            End If

            pbPoster.Invalidate()

        End If
    End Sub

    Private Shared Function ToInt(n As Double) As Integer
        Return CInt(Math.Round(n, MidpointRounding.AwayFromZero))
    End Function

    Private Shared Sub ClearImage(pictureBox As PictureBox, color As Color)
        If pictureBox.Image Is Nothing Then
            Dim bmp As New Bitmap(pictureBox.Width, pictureBox.Height)
            pictureBox.Image = bmp
        End If
        Dim g As Graphics = Graphics.FromImage(pictureBox.Image)
        g.Clear(color)
    End Sub

    Private Sub HandleUp()
        If ModifierKeys.HasFlag(Keys.Shift) Then
            Me._posterOffset.Y -= 10
        Else
            Me._posterOffset.Y -= 1
        End If
        DrawPoster()
    End Sub

    Private Sub HandleDown()
        If ModifierKeys.HasFlag(Keys.Shift) Then
            Me._posterOffset.Y += 10
        Else
            Me._posterOffset.Y += 1
        End If
        DrawPoster()
    End Sub

    Private Sub HandleRight()
        If ModifierKeys.HasFlag(Keys.Shift) Then
            Me._posterOffset.X += 10
        Else
            Me._posterOffset.X += 1
        End If
        DrawPoster()
    End Sub

    Private Sub HandleLeft()
        If ModifierKeys.HasFlag(Keys.Shift) Then
            Me._posterOffset.X -= 10
        Else
            Me._posterOffset.X -= 1
        End If
        DrawPoster()
    End Sub

    Private Sub HandleRestorePosition()
        Me._posterOffset.X = 0
        Me._posterOffset.Y = 0
        DrawPoster()
    End Sub

    Private Sub pbPoster_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles pbPoster.PreviewKeyDown
        If e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Or e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
            e.IsInputKey = True
        End If
    End Sub

    Private Sub ParentForm_PreviewKeyDown(ByVal sender As Object, ByVal e As PreviewKeyDownEventArgs)
        If e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Or e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
            e.IsInputKey = True
        End If
    End Sub

    Private Sub ParentForm_KeyDown(sender As Object, e As KeyEventArgs)
        If Not (pbPoster.Focused OrElse btnUp.Focused OrElse btnRight.Focused OrElse btnDown.Focused OrElse btnLeft.Focused OrElse btnRestorePosition.Focused) Then
            Return
        End If

        If e.KeyCode = Keys.Left Then
            HandleLeft()
            e.Handled = True
        ElseIf e.KeyCode = Keys.Right Then
            HandleRight()
            e.Handled = True
        ElseIf e.KeyCode = Keys.Up Then
            HandleUp()
            e.Handled = True
        ElseIf e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.A Then
            HandleDown()
            e.Handled = True
        End If
    End Sub

    Private Sub pnlMain_ParentChanged(sender As Object, e As EventArgs) Handles pnlMain.ParentChanged
        Dim parent = EditPanel.Parent
        While parent IsNot Nothing
            If TypeOf parent Is Form Then
                Dim parentForm = DirectCast(parent, Form)
                AddHandler parentForm.PreviewKeyDown, New PreviewKeyDownEventHandler(AddressOf Me.ParentForm_PreviewKeyDown)
                AddHandler parentForm.KeyDown, New KeyEventHandler(AddressOf Me.ParentForm_KeyDown)
                parentForm.KeyPreview = True
                Exit While
            End If
            parent = parent.Parent
        End While
    End Sub

    Private Sub pbPoster_Click(sender As Object, e As EventArgs) Handles pbPoster.Click
        pbPoster.Focus()
        pbPoster.Select()
    End Sub

    Private Sub cbCropEnabled_CheckedChanged(sender As Object, e As EventArgs) Handles cbCropEnabled.CheckedChanged
        _template.CropEnabled = cbCropEnabled.Checked
        cbShowCropBorder.Enabled = cbCropEnabled.Checked
        cbPreviewCropping.Enabled = cbCropEnabled.Checked
        cbAspectRatio.Enabled = cbCropEnabled.Checked
        DrawPoster()
    End Sub

    Private Sub pbPoster_DragDrop(sender As Object, e As DragEventArgs) Handles pbPoster.DragDrop
        'Dim tImage As MediaContainers.Image = FileUtils.DragAndDrop.GetDoppedImage(e)
        'If tImage.ImageOriginal.Image IsNot Nothing Then
        '    _tmpDBElement.ImagesContainer.Poster = tImage
        '    pbPoster.Image = _tmpDBElement.ImagesContainer.Poster.ImageOriginal.Image
        '    pbPoster.Tag = _tmpDBElement.ImagesContainer.Poster
        '    lblPosterSize.Text = String.Format(Master.eLang.GetString(269, "Size: {0}x{1}"), pbPoster.Image.Width, pbPoster.Image.Height)
        '    lblPosterSize.Visible = True
        'End If
    End Sub

    Private Sub pbPoster_DragEnter(sender As Object, e As DragEventArgs) Handles pbPoster.DragEnter
        'If FileUtils.DragAndDrop.CheckDroppedImage(e) Then
        '    e.Effect = DragDropEffects.Copy
        'Else
        '    e.Effect = DragDropEffects.None
        'End If
    End Sub

    Private Sub btnSaveAsPoster_Click(sender As Object, e As EventArgs) Handles btnSaveAsPoster.Click
        'FIXME: !!! handle the case when no _untouchedPoster (like ApplyTemplateToPoster method)
        Dim newPoster = PosterTemplateHelper.ApplyTemplateToBaseImage(_untouchedPoster, _tmpDBElement, _template, _overlayImage, _posterOffset, False)
        If newPoster IsNot Nothing Then
            Dim editedPosterTempPath = PosterTemplateHelper.SavePosterToTemp(newPoster)
            If editedPosterTempPath IsNot Nothing Then
                RaisePosterChangedEvent(editedPosterTempPath)
            End If
        End If
    End Sub

    Private Sub RaisePosterChangedEvent(editedPosterTempPath As String)
        _posterEditor.RaiseGenericEvent(Enums.ModuleEventType.ChangePosterDuringEdit_Movie, New List(Of Object)(New Object() {Me, Path.GetFileName(editedPosterTempPath)}))
    End Sub

    Private Sub cbOverlayEnabled_CheckedChanged(sender As Object, e As EventArgs) Handles cbOverlayEnabled.CheckedChanged
        Try
            SuspendDrawing()
            If cbOverlayEnabled.Checked AndAlso _overlayImage Is Nothing Then
                If _templatePath IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(_template.Image.Filename) Then
                    SetOverlayImageFromTemplate(New PosterTemplateWithPath(_template, _templatePath))
                End If
                If _overlayImage Is Nothing Then
                    ChangeOverlay()
                    If _overlayImage Is Nothing Then
                        _template.Image.Enabled = False
                        cbOverlayEnabled.Checked = False
                        Return
                    End If
                End If
            End If
            _template.Image.Enabled = cbOverlayEnabled.Checked

            rbAlignLeft.Enabled = cbOverlayEnabled.Checked
            rbAlignCenter.Enabled = cbOverlayEnabled.Checked
            rbAlignRight.Enabled = cbOverlayEnabled.Checked
            rbAlignStretch.Enabled = cbOverlayEnabled.Checked
            rbVAlignTop.Enabled = cbOverlayEnabled.Checked
            rbVAlignMiddle.Enabled = cbOverlayEnabled.Checked
            rbVAlignBottom.Enabled = cbOverlayEnabled.Checked
            rbVAlignStretch.Enabled = cbOverlayEnabled.Checked

            'in order to set correct txtOverlayWidth.Enabled status
            AlignChanged()

            DrawPoster()
        Finally
            ResumeDrawing()
        End Try
    End Sub

    Private Sub btnChangeOverlay_Click(sender As Object, e As EventArgs) Handles btnChangeOverlay.Click
        ChangeOverlay()
    End Sub

    Private Sub ChangeOverlay()
        With ofdLocalFiles
            .InitialDirectory = Directory.GetParent(_tmpDBElement.Filename).FullName
            .Filter = Master.eLang.GetString(497, "Images") + "|*.jpg;*.jpeg;*.png"
            .FilterIndex = 0
        End With

        If ofdLocalFiles.ShowDialog() = DialogResult.OK Then
            SetOverlayImage(ofdLocalFiles.FileName, False)
        End If
    End Sub

    Private Sub rbShowCropBorder_CheckedChanged(sender As Object, e As EventArgs)
        DrawPoster()
    End Sub

    Private Sub rbShowCropped_CheckedChanged(sender As Object, e As EventArgs)
        DrawPoster()
    End Sub

    Private Sub btnUp_Click(sender As Object, e As EventArgs) Handles btnUp.Click
        HandleUp()
    End Sub

    Private Sub btnDown_Click(sender As Object, e As EventArgs) Handles btnDown.Click
        HandleDown()
    End Sub

    Private Sub btnLeft_Click(sender As Object, e As EventArgs) Handles btnLeft.Click
        HandleLeft()
    End Sub

    Private Sub btnRight_Click(sender As Object, e As EventArgs) Handles btnRight.Click
        HandleRight()
    End Sub

    Private Sub btnRestorePosition_Click(sender As Object, e As EventArgs) Handles btnRestorePosition.Click
        HandleRestorePosition()
    End Sub

    Private Sub btnSaveTemplate_Click(sender As Object, e As EventArgs) Handles btnSaveTemplate.Click
        cmsSaveTemplate.Show(btnSaveTemplate, 0, btnSaveTemplate.Height)
    End Sub

    Private Sub cbAspectRatio_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbAspectRatio.SelectedIndexChanged
        _template.PosterRatio = DirectCast(cbAspectRatio.SelectedItem, PosterTemplate.AspectRatio)
        DrawPoster()
    End Sub

    Private Sub cbShowCropBorder_CheckedChanged(sender As Object, e As EventArgs) Handles cbShowCropBorder.CheckedChanged
        _drawSettings.ShowCropBorder = cbShowCropBorder.Checked
        DrawPoster()
    End Sub

    Private Sub cbPreviewCropping_CheckedChanged(sender As Object, e As EventArgs) Handles cbPreviewCropping.CheckedChanged
        Try
            SuspendDrawing()
            If Not cbPreviewCropping.Checked Then
                cbShowCropBorder.Checked = True
                cbShowCropBorder.Enabled = False
            Else
                cbShowCropBorder.Enabled = True
            End If
            _drawSettings.PreviewCropping = cbPreviewCropping.Checked
            DrawPoster()
        Finally
            ResumeDrawing()
        End Try
    End Sub

    Private Sub AlignChanged()
        DrawPoster()
        txtOverlayWidth.Enabled = Not rbAlignStretch.Checked AndAlso Not rbVAlignStretch.Checked
    End Sub

    Private Sub rbAlignLeft_CheckedChanged(sender As Object, e As EventArgs) Handles rbAlignLeft.CheckedChanged
        _template.Image.HorizontalAlignment = PosterTemplate.HorizontalAlignment.Left
        AlignChanged()
    End Sub

    Private Sub rbAlignCenter_CheckedChanged(sender As Object, e As EventArgs) Handles rbAlignCenter.CheckedChanged
        _template.Image.HorizontalAlignment = PosterTemplate.HorizontalAlignment.Center
        AlignChanged()
    End Sub

    Private Sub rbAlignRight_CheckedChanged(sender As Object, e As EventArgs) Handles rbAlignRight.CheckedChanged
        _template.Image.HorizontalAlignment = PosterTemplate.HorizontalAlignment.Right
        AlignChanged()
    End Sub

    Private Sub rbAlignStretch_CheckedChanged(sender As Object, e As EventArgs) Handles rbAlignStretch.CheckedChanged
        _template.Image.HorizontalAlignment = PosterTemplate.HorizontalAlignment.Stretch
        AlignChanged()
    End Sub

    Private Sub rbVAlignTop_CheckedChanged(sender As Object, e As EventArgs) Handles rbVAlignTop.CheckedChanged
        _template.Image.VerticalAlignment = PosterTemplate.VerticalAlignment.Top
        AlignChanged()
    End Sub

    Private Sub rbVAlignMiddle_CheckedChanged(sender As Object, e As EventArgs) Handles rbVAlignMiddle.CheckedChanged
        _template.Image.VerticalAlignment = PosterTemplate.VerticalAlignment.Middle
        AlignChanged()
    End Sub

    Private Sub rbVAlignBottom_CheckedChanged(sender As Object, e As EventArgs) Handles rbVAlignBottom.CheckedChanged
        _template.Image.VerticalAlignment = PosterTemplate.VerticalAlignment.Bottom
        AlignChanged()
    End Sub

    Private Sub rbVAlignStretch_CheckedChanged(sender As Object, e As EventArgs) Handles rbVAlignStretch.CheckedChanged
        _template.Image.VerticalAlignment = PosterTemplate.VerticalAlignment.Stretch
        AlignChanged()
    End Sub

    Private Sub txtOverlayWidth_TextChanged(sender As Object, e As EventArgs) Handles txtOverlayWidth.TextChanged
        Dim i As Double
        If Double.TryParse(txtOverlayWidth.Text, i) AndAlso i >= 0 OrElse i <= 100 Then
            If _template IsNot Nothing Then
                _template.Image.WidthPercent = i
                DrawPoster()
            End If
        Else
            MsgBox("Please insert a number between 0 and 100.")
            Return
        End If
    End Sub

    Private Sub Init_cmsSaveTemplate()
        If Not _cmsSaveTemplate_Inited Then
            'FIXME: i18n
            tsmiMovie.Text = "Save to Movie"
            AddHandler tsmiMovie.Click, New EventHandler(AddressOf Me.HandleSaveTemplateToMovie)

            'FIXME: i18n
            tsmiGallery.Text = "Save to Gallery"
            AddHandler tsmiGallery.Click, New EventHandler(AddressOf Me.HandleSaveTemplateToGallery)

            'FIXME: i18n
            tsmiContainingFolder.Text = "Save as Containing Folder default"

            'Cannot be changed by user, so we can do it in init
            FillContainingFolderMenuItems(_tmpDBElement, tsmiContainingFolder, New EventHandler(AddressOf Me.HandleSaveTemplateIntoContainingFolder))

            'FIXME: i18n
            tsmiStudio.Text = "Save as Studio default"
            'FIXME: i18n
            tsmiTag.Text = "Save as Tag default"
            _cmsSaveTemplate_Inited = True
        End If
    End Sub

    Private Sub cmsSaveTemplate_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsSaveTemplate.Opening
        Init_cmsSaveTemplate()
        FillStudioMenuItems(_tmpDBElement, tsmiStudio, New EventHandler(AddressOf HandleSaveAsStudioDefault))
        FillTagMenuItems(_tmpDBElement, tsmiTag, New EventHandler(AddressOf HandleSaveAsTagDefault))
    End Sub

    Private Shared Sub FillContainingFolderMenuItems(dbElement As Database.DBElement, tsmiParent As ToolStripMenuItem, clickHandler As EventHandler)
        tsmiParent.DropDownItems.Clear()
        Dim directoryPath As String = Path.GetDirectoryName(dbElement.Filename)
        While FileUtils.Common.IsAscendantOrTheSamePath(directoryPath, dbElement.Source.Path)
            Dim item = New ToolStripMenuItem()
            item.Text = directoryPath
            item.Tag = directoryPath
            AddHandler item.Click, clickHandler
            tsmiParent.DropDownItems.Add(item)
            directoryPath = Path.GetDirectoryName(directoryPath)
        End While
    End Sub

    Private Shared Sub FillTagMenuItems(dbElement As Database.DBElement, tsmiParent As ToolStripMenuItem, clickHandler As EventHandler)
        tsmiParent.DropDownItems.Clear()
        If dbElement.Movie.Tags.Count > 0 Then
            For Each movieTag In dbElement.Movie.Tags
                Dim item = New ToolStripMenuItem()
                item.Text = movieTag
                AddHandler item.Click, clickHandler
                tsmiParent.DropDownItems.Add(item)
            Next
        Else
            Dim item = New ToolStripMenuItem()
            'FIXME: i18n
            item.Text = "Movie has no tags."
            item.Enabled = False
            tsmiParent.DropDownItems.Add(item)
        End If
    End Sub

    Private Shared Sub FillStudioMenuItems(dbElement As Database.DBElement, tsmiParent As ToolStripMenuItem, clickHandler As EventHandler)
        tsmiParent.DropDownItems.Clear()
        If dbElement.Movie.Studios.Count > 0 Then
            For Each studio In dbElement.Movie.Studios
                Dim item = New ToolStripMenuItem()
                item.Text = studio
                item.Tag = studio
                AddHandler item.Click, clickHandler
                tsmiParent.DropDownItems.Add(item)
            Next
        Else
            Dim item = New ToolStripMenuItem()
            'FIXME: i18n
            item.Text = "Movie has no studios."
            item.Enabled = False
            tsmiParent.DropDownItems.Add(item)
        End If
    End Sub

    Private Sub HandleSaveTemplateToMovie(sender As Object, e As EventArgs)
        PosterTemplateHelper.SaveTemplateToMovie(_tmpDBElement, _template, _overlayImagePath, True)
    End Sub

    Private Sub HandleSaveAsTagDefault(sender As Object, e As EventArgs)
        'FIXME: implement
        MsgBox("Not implemented in beta version.")
    End Sub

    Private Sub HandleSaveTemplateIntoContainingFolder(sender As Object, e As EventArgs)
        Dim folder As String = DirectCast(sender, ToolStripMenuItem).Tag.ToString

        PosterTemplateHelper.SaveTemplateIntoContainingFolder(folder, _template, _overlayImagePath, True)
    End Sub

    Private Sub HandleSaveAsStudioDefault(sender As Object, e As EventArgs)
        'FIXME: implement
        MsgBox("Not implemented in beta version.")
    End Sub

    Private Sub HandleSaveTemplateToGallery(sender As Object, e As EventArgs)
        'FIXME: implement
        MsgBox("Not implemented in beta version.")
    End Sub


    Private Sub tbFrameExtractPercent_Scroll(sender As Object, e As EventArgs) Handles tbFrameExtractPercent.Scroll
        Try
            txtFrameExtractPercent.Text = String.Format("{0}", tbFrameExtractPercent.Value)
        Catch ex As Exception
            logger.Error(ex, New StackFrame().GetMethod().Name)
        End Try

    End Sub

    Private Sub txtFrameExtractPercent_TextChanged(sender As Object, e As EventArgs) Handles txtFrameExtractPercent.TextChanged
        Try
            Dim i = Integer.Parse(txtFrameExtractPercent.Text)
            If i < 0 OrElse i > 100 Then
                'FIXME: i18n
                MsgBox("Please insert a valid integer between 0 and 100.")
                Return
            End If
            tbFrameExtractPercent.Value = i
            'FIXME: redraw poster
            'DrawPoster()
        Catch ex As Exception
            'FIXME: i18n
            MsgBox("Please insert a valid integer between 0 and 100.")
        End Try
    End Sub

    Private Sub cbExtractFrame_CheckedChanged(sender As Object, e As EventArgs) Handles cbExtractFrame.CheckedChanged
        tbFrameExtractPercent.Enabled = cbExtractFrame.Checked
        txtFrameExtractPercent.Enabled = cbExtractFrame.Checked
    End Sub

    Private Sub btnApplyTemplate_Click(sender As Object, e As EventArgs) Handles btnApplyTemplate.Click
        cmsApplyTemplate.Show(btnApplyTemplate, 0, btnApplyTemplate.Height)
    End Sub

    Private Sub Init_cmsApplyTemplate()
        If Not _cmsApplyTemplate_Inited Then
            'FIXME: internationalization 
            tsmiApplyToContainingFolder.Text = "Apply to Containing Folder"

            'Cannot be changed by user, so we can do it in init
            FillContainingFolderMenuItems(_tmpDBElement, tsmiApplyToContainingFolder, New EventHandler(AddressOf Me.HandleApplyTemplateToContainingFolder))
            'FIXME: internationalization 
            tsmiApplyToStudio.Text = "Apply to Studio"
            'FIXME: internationalization 
            tsmiApplyToTag.Text = "Apply to Tag"

            _cmsSaveTemplate_Inited = True
        End If
    End Sub

    Private Sub cmsApplyTemplate_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsSaveTemplate.Opening, cmsApplyTemplate.Opening
        Init_cmsApplyTemplate()
        FillStudioMenuItems(_tmpDBElement, tsmiApplyToStudio, New EventHandler(AddressOf HandleApplyToStudio))
        FillTagMenuItems(_tmpDBElement, tsmiTag, New EventHandler(AddressOf HandleApplyToTag))
    End Sub

    Private Sub HandleApplyTemplateToContainingFolder(sender As Object, e As EventArgs)
        Using dialog = New dlgApplyTemplate(_posterEditor)
            Dim changedMovieIDs = dialog.ShowToContainigFolder(DirectCast(sender, ToolStripMenuItem).Tag.ToString, _template, _overlayImage)
            HandleMovieChanged(changedMovieIDs)
        End Using
    End Sub

    Private Sub HandleApplyToStudio(sender As Object, e As EventArgs)
        Using dialog = New dlgApplyTemplate(_posterEditor)
            Dim changedMovieIDs = dialog.ShowForStudio(DirectCast(sender, ToolStripMenuItem).Tag.ToString, _template, _overlayImage)
            HandleMovieChanged(changedMovieIDs)
        End Using
    End Sub

    Private Sub HandleApplyToTag(sender As Object, e As EventArgs)
        'FIXME: implement
        MsgBox("Not implemented in beta version.")
    End Sub

    Private Sub HandleMovieChanged(chengedMovieIDs As List(Of Long))
        If chengedMovieIDs.Contains(_tmpDBElement.ID) Then
            Dim path As String = Nothing
            Try
                _tmpDBElement = Master.DB.Load_Movie(_tmpDBElement.ID)
                _tmpDBElement.ImagesContainer.Poster.LoadAndCache(Enums.ContentType.Movie, True, True)
                path = PosterTemplateHelper.SavePosterToTemp(_tmpDBElement.ImagesContainer.Poster.ImageOriginal.Image)
                RaisePosterChangedEvent(path)
                DrawPoster()
            Finally
                PosterTemplateHelper.SafeDelete(path)
            End Try
        End If
    End Sub

    Private Sub btnOpenImage_Click(sender As Object, e As EventArgs) Handles btnOpenImage.Click
        If Not ModulesManager.Instance.ScraperWithCapabilityAnyEnabled_Image_Movie(Enums.ModifierType.MainPoster) Then
            'btnSetPosterScrape.Enabled = False
        End If
    End Sub

    Private Sub btnReloadUntouched_Click(sender As Object, e As EventArgs) Handles btnReloadUntouched.Click
        Dim path As String = Nothing
        Try
            path = PosterTemplateHelper.GetExistingUntouchedBaseFilePathForMovie(_tmpDBElement)
            If path Is Nothing Then
                btnReloadUntouched.Enabled = False
                'FIXME: i18n
                MsgBox("Untouched base image doesn't exist.")
                Return
            End If
            Dim newBaseImage = Image.FromFile(path)
            'while the file is opened couldn't be overwritten (we'd try it during poster saving in ApplyTemplateToBaseImage() method)
            newBaseImage.Tag = New ImageFileInfo(path)

            DisposeBaseImage()
            _untouchedPoster = newBaseImage
        Catch ex As Exception
            logger.Error(ex, "Cannot load base image: {0}", path)
            'FIXME: i18n
            MsgBox(String.Format("Cannot load image."))
        Finally
            DrawPoster()
        End Try
    End Sub

    Private Sub DisposeBaseImage()
        'we don't do it, because it could be dangerous (eg. we starting to use ImagesContainer.Fanart az baseimage), so we let gc do its work
        'If _untouchedPoster IsNot Nothing AndAlso _untouchedPoster Is _tmpDBElement.ImagesContainer.Poster.ImageOriginal.Image Then
        '    PosterTemplateHelper.SafeDispose(_untouchedPoster)
        '    _untouchedPoster = Nothing
        'End If
    End Sub

    Private Sub txtOverlayWidth_KeyDown(sender As Object, e As KeyEventArgs) Handles txtOverlayWidth.KeyDown
        If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            Dim newValue As Double
            Dim change As Double
            If e.Control AndAlso e.Shift Then
                change = 0.01
            ElseIf e.Shift Then
                change = 0.1
            Else
                change = 1
            End If
            If Double.TryParse(txtOverlayWidth.Text, newValue) Then
                If e.KeyCode = Keys.Up Then
                    newValue += change
                ElseIf e.KeyCode = Keys.Down Then
                    newValue -= change
                End If
                If newValue < 0 Then
                    newValue = 0
                ElseIf newValue > 100 Then
                    newValue = 100
                End If
            End If
            SetOverlayWidthText(newValue)
            e.Handled = True
        End If
    End Sub

#End Region 'Methods



End Class
