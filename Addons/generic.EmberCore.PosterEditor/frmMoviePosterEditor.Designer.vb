<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMoviePosterEditor
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMoviePosterEditor))
        Me.pnlMain = New System.Windows.Forms.Panel()
        Me.btnOpenImage = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.tpGeneral = New System.Windows.Forms.TabPage()
        Me.grbCrop = New System.Windows.Forms.GroupBox()
        Me.cbPreviewCropping = New System.Windows.Forms.CheckBox()
        Me.cbShowCropBorder = New System.Windows.Forms.CheckBox()
        Me.btnActorAdd = New System.Windows.Forms.Button()
        Me.lblAspectRatio = New System.Windows.Forms.Label()
        Me.cbAspectRatio = New System.Windows.Forms.ComboBox()
        Me.cbCropEnabled = New System.Windows.Forms.CheckBox()
        Me.grbOverlay = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblOverlayWidth = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.rbAlignStretch = New System.Windows.Forms.RadioButton()
        Me.rbAlignRight = New System.Windows.Forms.RadioButton()
        Me.rbAlignCenter = New System.Windows.Forms.RadioButton()
        Me.rbAlignLeft = New System.Windows.Forms.RadioButton()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.rbVAlignStretch = New System.Windows.Forms.RadioButton()
        Me.rbVAlignBottom = New System.Windows.Forms.RadioButton()
        Me.rbVAlignMiddle = New System.Windows.Forms.RadioButton()
        Me.rbVAlignTop = New System.Windows.Forms.RadioButton()
        Me.txtOverlayWidth = New System.Windows.Forms.TextBox()
        Me.btnChangeOverlay = New System.Windows.Forms.Button()
        Me.cbOverlayEnabled = New System.Windows.Forms.CheckBox()
        Me.tpBaseImage = New System.Windows.Forms.TabPage()
        Me.lblBaseImageSize = New System.Windows.Forms.Label()
        Me.lblFramePosition = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtBaseImageSize = New System.Windows.Forms.TextBox()
        Me.tbBaseImageSize = New System.Windows.Forms.TrackBar()
        Me.cbExtractFrame = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtFrameExtractPercent = New System.Windows.Forms.TextBox()
        Me.tbFrameExtractPercent = New System.Windows.Forms.TrackBar()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnApplyTemplate = New System.Windows.Forms.Button()
        Me.btnSaveTemplate = New System.Windows.Forms.Button()
        Me.cmsSaveTemplate = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmiMovie = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiContainingFolder = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiGallery = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiStudio = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiTag = New System.Windows.Forms.ToolStripMenuItem()
        Me.btnOpenTemplate = New System.Windows.Forms.Button()
        Me.btnSaveAsPoster = New System.Windows.Forms.Button()
        Me.btnRestorePosition = New System.Windows.Forms.Button()
        Me.btnRight = New System.Windows.Forms.Button()
        Me.btnLeft = New System.Windows.Forms.Button()
        Me.btnDown = New System.Windows.Forms.Button()
        Me.btnUp = New System.Windows.Forms.Button()
        Me.lblPosterSize = New System.Windows.Forms.Label()
        Me.btnReloadUntouched = New System.Windows.Forms.Button()
        Me.pbPoster = New System.Windows.Forms.PictureBox()
        Me.ofdLocalFiles = New System.Windows.Forms.OpenFileDialog()
        Me.cmsApplyTemplate = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmiApplyToContainingFolder = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem6 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiApplyToStudio = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiApplyToTag = New System.Windows.Forms.ToolStripMenuItem()
        Me.ttpForm = New System.Windows.Forms.ToolTip(Me.components)
        Me.lblBaseImageRotate = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtBaseImageRotate = New System.Windows.Forms.TextBox()
        Me.tbBaseImageRotate = New System.Windows.Forms.TrackBar()
        Me.pnlMain.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.tpGeneral.SuspendLayout()
        Me.grbCrop.SuspendLayout()
        Me.grbOverlay.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.tpBaseImage.SuspendLayout()
        CType(Me.tbBaseImageSize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbFrameExtractPercent, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.cmsSaveTemplate.SuspendLayout()
        CType(Me.pbPoster, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmsApplyTemplate.SuspendLayout()
        CType(Me.tbBaseImageRotate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlMain
        '
        Me.pnlMain.BackColor = System.Drawing.Color.White
        Me.pnlMain.Controls.Add(Me.btnOpenImage)
        Me.pnlMain.Controls.Add(Me.TabControl1)
        Me.pnlMain.Controls.Add(Me.GroupBox1)
        Me.pnlMain.Controls.Add(Me.btnSaveAsPoster)
        Me.pnlMain.Controls.Add(Me.btnRestorePosition)
        Me.pnlMain.Controls.Add(Me.btnRight)
        Me.pnlMain.Controls.Add(Me.btnLeft)
        Me.pnlMain.Controls.Add(Me.btnDown)
        Me.pnlMain.Controls.Add(Me.btnUp)
        Me.pnlMain.Controls.Add(Me.lblPosterSize)
        Me.pnlMain.Controls.Add(Me.btnReloadUntouched)
        Me.pnlMain.Controls.Add(Me.pbPoster)
        Me.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMain.Location = New System.Drawing.Point(0, 0)
        Me.pnlMain.Name = "pnlMain"
        Me.pnlMain.Size = New System.Drawing.Size(990, 485)
        Me.pnlMain.TabIndex = 0
        '
        'btnOpenImage
        '
        Me.btnOpenImage.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.btnOpenImage.Image = CType(resources.GetObject("btnOpenImage.Image"), System.Drawing.Image)
        Me.btnOpenImage.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnOpenImage.Location = New System.Drawing.Point(108, 398)
        Me.btnOpenImage.Name = "btnOpenImage"
        Me.btnOpenImage.Size = New System.Drawing.Size(96, 83)
        Me.btnOpenImage.TabIndex = 22
        Me.btnOpenImage.Text = "Open Image "
        Me.btnOpenImage.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnOpenImage.UseVisualStyleBackColor = True
        Me.btnOpenImage.Visible = False
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.tpGeneral)
        Me.TabControl1.Controls.Add(Me.tpBaseImage)
        Me.TabControl1.Location = New System.Drawing.Point(698, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(292, 394)
        Me.TabControl1.TabIndex = 21
        '
        'tpGeneral
        '
        Me.tpGeneral.Controls.Add(Me.grbCrop)
        Me.tpGeneral.Controls.Add(Me.grbOverlay)
        Me.tpGeneral.Location = New System.Drawing.Point(4, 22)
        Me.tpGeneral.Name = "tpGeneral"
        Me.tpGeneral.Padding = New System.Windows.Forms.Padding(3)
        Me.tpGeneral.Size = New System.Drawing.Size(284, 368)
        Me.tpGeneral.TabIndex = 0
        Me.tpGeneral.Text = "tpGeneral"
        Me.tpGeneral.UseVisualStyleBackColor = True
        '
        'grbCrop
        '
        Me.grbCrop.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbCrop.Controls.Add(Me.cbPreviewCropping)
        Me.grbCrop.Controls.Add(Me.cbShowCropBorder)
        Me.grbCrop.Controls.Add(Me.btnActorAdd)
        Me.grbCrop.Controls.Add(Me.lblAspectRatio)
        Me.grbCrop.Controls.Add(Me.cbAspectRatio)
        Me.grbCrop.Controls.Add(Me.cbCropEnabled)
        Me.grbCrop.Location = New System.Drawing.Point(5, 6)
        Me.grbCrop.Name = "grbCrop"
        Me.grbCrop.Size = New System.Drawing.Size(273, 154)
        Me.grbCrop.TabIndex = 16
        Me.grbCrop.TabStop = False
        Me.grbCrop.Text = "Crop"
        '
        'cbPreviewCropping
        '
        Me.cbPreviewCropping.AutoSize = True
        Me.cbPreviewCropping.Location = New System.Drawing.Point(6, 124)
        Me.cbPreviewCropping.Name = "cbPreviewCropping"
        Me.cbPreviewCropping.Size = New System.Drawing.Size(117, 17)
        Me.cbPreviewCropping.TabIndex = 34
        Me.cbPreviewCropping.Text = "Preview Cropping"
        Me.cbPreviewCropping.UseVisualStyleBackColor = True
        '
        'cbShowCropBorder
        '
        Me.cbShowCropBorder.AutoSize = True
        Me.cbShowCropBorder.Location = New System.Drawing.Point(7, 101)
        Me.cbShowCropBorder.Name = "cbShowCropBorder"
        Me.cbShowCropBorder.Size = New System.Drawing.Size(120, 17)
        Me.cbShowCropBorder.TabIndex = 33
        Me.cbShowCropBorder.Text = "Show Crop Border"
        Me.cbShowCropBorder.UseVisualStyleBackColor = True
        '
        'btnActorAdd
        '
        Me.btnActorAdd.Image = CType(resources.GetObject("btnActorAdd.Image"), System.Drawing.Image)
        Me.btnActorAdd.Location = New System.Drawing.Point(242, 73)
        Me.btnActorAdd.Name = "btnActorAdd"
        Me.btnActorAdd.Size = New System.Drawing.Size(23, 23)
        Me.btnActorAdd.TabIndex = 32
        Me.btnActorAdd.UseVisualStyleBackColor = True
        '
        'lblAspectRatio
        '
        Me.lblAspectRatio.AutoSize = True
        Me.lblAspectRatio.Location = New System.Drawing.Point(4, 57)
        Me.lblAspectRatio.Name = "lblAspectRatio"
        Me.lblAspectRatio.Size = New System.Drawing.Size(71, 13)
        Me.lblAspectRatio.TabIndex = 2
        Me.lblAspectRatio.Text = "Aspect ratio:"
        '
        'cbAspectRatio
        '
        Me.cbAspectRatio.FormattingEnabled = True
        Me.cbAspectRatio.Items.AddRange(New Object() {"1 : 1.5", "1 : 1"})
        Me.cbAspectRatio.Location = New System.Drawing.Point(6, 73)
        Me.cbAspectRatio.Name = "cbAspectRatio"
        Me.cbAspectRatio.Size = New System.Drawing.Size(230, 21)
        Me.cbAspectRatio.TabIndex = 1
        '
        'cbCropEnabled
        '
        Me.cbCropEnabled.AutoSize = True
        Me.cbCropEnabled.Location = New System.Drawing.Point(6, 21)
        Me.cbCropEnabled.Name = "cbCropEnabled"
        Me.cbCropEnabled.Size = New System.Drawing.Size(68, 17)
        Me.cbCropEnabled.TabIndex = 0
        Me.cbCropEnabled.Text = "Enabled"
        Me.cbCropEnabled.UseVisualStyleBackColor = True
        '
        'grbOverlay
        '
        Me.grbOverlay.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbOverlay.Controls.Add(Me.Label2)
        Me.grbOverlay.Controls.Add(Me.lblOverlayWidth)
        Me.grbOverlay.Controls.Add(Me.Panel2)
        Me.grbOverlay.Controls.Add(Me.Panel1)
        Me.grbOverlay.Controls.Add(Me.txtOverlayWidth)
        Me.grbOverlay.Controls.Add(Me.btnChangeOverlay)
        Me.grbOverlay.Controls.Add(Me.cbOverlayEnabled)
        Me.grbOverlay.Location = New System.Drawing.Point(6, 166)
        Me.grbOverlay.Name = "grbOverlay"
        Me.grbOverlay.Size = New System.Drawing.Size(272, 221)
        Me.grbOverlay.TabIndex = 17
        Me.grbOverlay.TabStop = False
        Me.grbOverlay.Text = "Overlay"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(115, 143)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(17, 13)
        Me.Label2.TabIndex = 27
        Me.Label2.Text = "%"
        '
        'lblOverlayWidth
        '
        Me.lblOverlayWidth.AutoSize = True
        Me.lblOverlayWidth.Location = New System.Drawing.Point(7, 124)
        Me.lblOverlayWidth.Name = "lblOverlayWidth"
        Me.lblOverlayWidth.Size = New System.Drawing.Size(42, 13)
        Me.lblOverlayWidth.TabIndex = 26
        Me.lblOverlayWidth.Text = "Width:"
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.rbAlignStretch)
        Me.Panel2.Controls.Add(Me.rbAlignRight)
        Me.Panel2.Controls.Add(Me.rbAlignCenter)
        Me.Panel2.Controls.Add(Me.rbAlignLeft)
        Me.Panel2.Location = New System.Drawing.Point(144, 51)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(116, 34)
        Me.Panel2.TabIndex = 25
        '
        'rbAlignStretch
        '
        Me.rbAlignStretch.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbAlignStretch.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.align_stretch_16x16
        Me.rbAlignStretch.Location = New System.Drawing.Point(90, 7)
        Me.rbAlignStretch.Name = "rbAlignStretch"
        Me.rbAlignStretch.Size = New System.Drawing.Size(23, 23)
        Me.rbAlignStretch.TabIndex = 3
        Me.rbAlignStretch.UseVisualStyleBackColor = True
        '
        'rbAlignRight
        '
        Me.rbAlignRight.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbAlignRight.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.align_right_16x16
        Me.rbAlignRight.Location = New System.Drawing.Point(61, 7)
        Me.rbAlignRight.Name = "rbAlignRight"
        Me.rbAlignRight.Size = New System.Drawing.Size(23, 23)
        Me.rbAlignRight.TabIndex = 2
        Me.rbAlignRight.UseVisualStyleBackColor = True
        '
        'rbAlignCenter
        '
        Me.rbAlignCenter.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbAlignCenter.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.align_center_16x16
        Me.rbAlignCenter.Location = New System.Drawing.Point(32, 7)
        Me.rbAlignCenter.Name = "rbAlignCenter"
        Me.rbAlignCenter.Size = New System.Drawing.Size(23, 23)
        Me.rbAlignCenter.TabIndex = 1
        Me.rbAlignCenter.UseVisualStyleBackColor = True
        '
        'rbAlignLeft
        '
        Me.rbAlignLeft.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbAlignLeft.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.align_left_16x16
        Me.rbAlignLeft.Location = New System.Drawing.Point(3, 7)
        Me.rbAlignLeft.Name = "rbAlignLeft"
        Me.rbAlignLeft.Size = New System.Drawing.Size(23, 23)
        Me.rbAlignLeft.TabIndex = 0
        Me.rbAlignLeft.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.rbVAlignStretch)
        Me.Panel1.Controls.Add(Me.rbVAlignBottom)
        Me.Panel1.Controls.Add(Me.rbVAlignMiddle)
        Me.Panel1.Controls.Add(Me.rbVAlignTop)
        Me.Panel1.Location = New System.Drawing.Point(144, 82)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(116, 34)
        Me.Panel1.TabIndex = 24
        '
        'rbVAlignStretch
        '
        Me.rbVAlignStretch.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbVAlignStretch.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.valign_stretch_16x16
        Me.rbVAlignStretch.Location = New System.Drawing.Point(90, 7)
        Me.rbVAlignStretch.Name = "rbVAlignStretch"
        Me.rbVAlignStretch.Size = New System.Drawing.Size(23, 23)
        Me.rbVAlignStretch.TabIndex = 3
        Me.rbVAlignStretch.UseVisualStyleBackColor = True
        '
        'rbVAlignBottom
        '
        Me.rbVAlignBottom.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbVAlignBottom.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.valign_bottom_16x16
        Me.rbVAlignBottom.Location = New System.Drawing.Point(61, 7)
        Me.rbVAlignBottom.Name = "rbVAlignBottom"
        Me.rbVAlignBottom.Size = New System.Drawing.Size(23, 23)
        Me.rbVAlignBottom.TabIndex = 2
        Me.rbVAlignBottom.UseVisualStyleBackColor = True
        '
        'rbVAlignMiddle
        '
        Me.rbVAlignMiddle.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbVAlignMiddle.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.valign_middle_16x16
        Me.rbVAlignMiddle.Location = New System.Drawing.Point(32, 7)
        Me.rbVAlignMiddle.Name = "rbVAlignMiddle"
        Me.rbVAlignMiddle.Size = New System.Drawing.Size(23, 23)
        Me.rbVAlignMiddle.TabIndex = 1
        Me.rbVAlignMiddle.UseVisualStyleBackColor = True
        '
        'rbVAlignTop
        '
        Me.rbVAlignTop.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbVAlignTop.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.valign_top_16x16
        Me.rbVAlignTop.Location = New System.Drawing.Point(3, 7)
        Me.rbVAlignTop.Name = "rbVAlignTop"
        Me.rbVAlignTop.Size = New System.Drawing.Size(23, 23)
        Me.rbVAlignTop.TabIndex = 0
        Me.rbVAlignTop.UseVisualStyleBackColor = True
        '
        'txtOverlayWidth
        '
        Me.txtOverlayWidth.Location = New System.Drawing.Point(6, 140)
        Me.txtOverlayWidth.Name = "txtOverlayWidth"
        Me.txtOverlayWidth.Size = New System.Drawing.Size(103, 22)
        Me.txtOverlayWidth.TabIndex = 23
        Me.txtOverlayWidth.Text = "100.00"
        Me.txtOverlayWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'btnChangeOverlay
        '
        Me.btnChangeOverlay.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.btnChangeOverlay.Image = CType(resources.GetObject("btnChangeOverlay.Image"), System.Drawing.Image)
        Me.btnChangeOverlay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnChangeOverlay.Location = New System.Drawing.Point(6, 58)
        Me.btnChangeOverlay.Name = "btnChangeOverlay"
        Me.btnChangeOverlay.Size = New System.Drawing.Size(126, 54)
        Me.btnChangeOverlay.TabIndex = 8
        Me.btnChangeOverlay.Text = "Change"
        Me.btnChangeOverlay.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnChangeOverlay.UseVisualStyleBackColor = True
        '
        'cbOverlayEnabled
        '
        Me.cbOverlayEnabled.AutoSize = True
        Me.cbOverlayEnabled.Location = New System.Drawing.Point(7, 22)
        Me.cbOverlayEnabled.Name = "cbOverlayEnabled"
        Me.cbOverlayEnabled.Size = New System.Drawing.Size(68, 17)
        Me.cbOverlayEnabled.TabIndex = 0
        Me.cbOverlayEnabled.Text = "Enabled"
        Me.cbOverlayEnabled.UseVisualStyleBackColor = True
        '
        'tpBaseImage
        '
        Me.tpBaseImage.Controls.Add(Me.lblBaseImageRotate)
        Me.tpBaseImage.Controls.Add(Me.Label5)
        Me.tpBaseImage.Controls.Add(Me.txtBaseImageRotate)
        Me.tpBaseImage.Controls.Add(Me.tbBaseImageRotate)
        Me.tpBaseImage.Controls.Add(Me.lblBaseImageSize)
        Me.tpBaseImage.Controls.Add(Me.lblFramePosition)
        Me.tpBaseImage.Controls.Add(Me.Label3)
        Me.tpBaseImage.Controls.Add(Me.txtBaseImageSize)
        Me.tpBaseImage.Controls.Add(Me.tbBaseImageSize)
        Me.tpBaseImage.Controls.Add(Me.cbExtractFrame)
        Me.tpBaseImage.Controls.Add(Me.Label1)
        Me.tpBaseImage.Controls.Add(Me.txtFrameExtractPercent)
        Me.tpBaseImage.Controls.Add(Me.tbFrameExtractPercent)
        Me.tpBaseImage.Location = New System.Drawing.Point(4, 22)
        Me.tpBaseImage.Name = "tpBaseImage"
        Me.tpBaseImage.Padding = New System.Windows.Forms.Padding(3)
        Me.tpBaseImage.Size = New System.Drawing.Size(284, 368)
        Me.tpBaseImage.TabIndex = 1
        Me.tpBaseImage.Text = "tpBaseImage"
        Me.tpBaseImage.UseVisualStyleBackColor = True
        '
        'lblBaseImageSize
        '
        Me.lblBaseImageSize.AutoSize = True
        Me.lblBaseImageSize.Location = New System.Drawing.Point(9, 108)
        Me.lblBaseImageSize.Name = "lblBaseImageSize"
        Me.lblBaseImageSize.Size = New System.Drawing.Size(27, 13)
        Me.lblBaseImageSize.TabIndex = 8
        Me.lblBaseImageSize.Text = "Size"
        '
        'lblFramePosition
        '
        Me.lblFramePosition.AutoSize = True
        Me.lblFramePosition.Location = New System.Drawing.Point(9, 52)
        Me.lblFramePosition.Name = "lblFramePosition"
        Me.lblFramePosition.Size = New System.Drawing.Size(84, 13)
        Me.lblFramePosition.TabIndex = 7
        Me.lblFramePosition.Text = "Frame position"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(264, 130)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(17, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "%"
        '
        'txtBaseImageSize
        '
        Me.txtBaseImageSize.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBaseImageSize.Location = New System.Drawing.Point(217, 127)
        Me.txtBaseImageSize.Name = "txtBaseImageSize"
        Me.txtBaseImageSize.Size = New System.Drawing.Size(46, 22)
        Me.txtBaseImageSize.TabIndex = 5
        Me.txtBaseImageSize.Text = "1000.00"
        '
        'tbBaseImageSize
        '
        Me.tbBaseImageSize.BackColor = System.Drawing.Color.White
        Me.tbBaseImageSize.Location = New System.Drawing.Point(3, 127)
        Me.tbBaseImageSize.Maximum = 1000
        Me.tbBaseImageSize.Name = "tbBaseImageSize"
        Me.tbBaseImageSize.Size = New System.Drawing.Size(208, 45)
        Me.tbBaseImageSize.TabIndex = 4
        Me.tbBaseImageSize.TickStyle = System.Windows.Forms.TickStyle.None
        Me.tbBaseImageSize.Value = 100
        '
        'cbExtractFrame
        '
        Me.cbExtractFrame.AutoSize = True
        Me.cbExtractFrame.Location = New System.Drawing.Point(11, 14)
        Me.cbExtractFrame.Name = "cbExtractFrame"
        Me.cbExtractFrame.Size = New System.Drawing.Size(155, 17)
        Me.cbExtractFrame.TabIndex = 3
        Me.cbExtractFrame.Text = "Extract frame if no poster"
        Me.cbExtractFrame.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(264, 75)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(17, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "%"
        '
        'txtFrameExtractPercent
        '
        Me.txtFrameExtractPercent.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFrameExtractPercent.Location = New System.Drawing.Point(217, 72)
        Me.txtFrameExtractPercent.Name = "txtFrameExtractPercent"
        Me.txtFrameExtractPercent.Size = New System.Drawing.Size(46, 22)
        Me.txtFrameExtractPercent.TabIndex = 1
        Me.txtFrameExtractPercent.Text = "100"
        '
        'tbFrameExtractPercent
        '
        Me.tbFrameExtractPercent.BackColor = System.Drawing.Color.White
        Me.tbFrameExtractPercent.Location = New System.Drawing.Point(3, 72)
        Me.tbFrameExtractPercent.Maximum = 100
        Me.tbFrameExtractPercent.Name = "tbFrameExtractPercent"
        Me.tbFrameExtractPercent.Size = New System.Drawing.Size(208, 45)
        Me.tbFrameExtractPercent.TabIndex = 0
        Me.tbFrameExtractPercent.TickStyle = System.Windows.Forms.TickStyle.None
        Me.tbFrameExtractPercent.Value = 50
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnApplyTemplate)
        Me.GroupBox1.Controls.Add(Me.btnSaveTemplate)
        Me.GroupBox1.Controls.Add(Me.btnOpenTemplate)
        Me.GroupBox1.Location = New System.Drawing.Point(707, 398)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(279, 84)
        Me.GroupBox1.TabIndex = 20
        Me.GroupBox1.TabStop = False
        '
        'btnApplyTemplate
        '
        Me.btnApplyTemplate.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnApplyTemplate.Image = CType(resources.GetObject("btnApplyTemplate.Image"), System.Drawing.Image)
        Me.btnApplyTemplate.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnApplyTemplate.Location = New System.Drawing.Point(199, 18)
        Me.btnApplyTemplate.Name = "btnApplyTemplate"
        Me.btnApplyTemplate.Size = New System.Drawing.Size(66, 60)
        Me.btnApplyTemplate.TabIndex = 22
        Me.btnApplyTemplate.Text = "Apply Template"
        Me.btnApplyTemplate.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnApplyTemplate.UseVisualStyleBackColor = True
        '
        'btnSaveTemplate
        '
        Me.btnSaveTemplate.ContextMenuStrip = Me.cmsSaveTemplate
        Me.btnSaveTemplate.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSaveTemplate.Image = CType(resources.GetObject("btnSaveTemplate.Image"), System.Drawing.Image)
        Me.btnSaveTemplate.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnSaveTemplate.Location = New System.Drawing.Point(117, 18)
        Me.btnSaveTemplate.Name = "btnSaveTemplate"
        Me.btnSaveTemplate.Size = New System.Drawing.Size(66, 60)
        Me.btnSaveTemplate.TabIndex = 21
        Me.btnSaveTemplate.Text = "Save Template"
        Me.btnSaveTemplate.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnSaveTemplate.UseVisualStyleBackColor = True
        '
        'cmsSaveTemplate
        '
        Me.cmsSaveTemplate.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiMovie, Me.tsmiContainingFolder, Me.tsmiGallery, Me.tsmiStudio, Me.tsmiTag})
        Me.cmsSaveTemplate.Name = "cmsSaveTemplate"
        Me.cmsSaveTemplate.Size = New System.Drawing.Size(211, 114)
        '
        'tsmiMovie
        '
        Me.tsmiMovie.Name = "tsmiMovie"
        Me.tsmiMovie.Size = New System.Drawing.Size(210, 22)
        Me.tsmiMovie.Text = "Save to Movie"
        '
        'tsmiContainingFolder
        '
        Me.tsmiContainingFolder.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem3, Me.ToolStripMenuItem4})
        Me.tsmiContainingFolder.Name = "tsmiContainingFolder"
        Me.tsmiContainingFolder.Size = New System.Drawing.Size(210, 22)
        Me.tsmiContainingFolder.Text = "Save to Containing Folder"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(116, 22)
        Me.ToolStripMenuItem3.Text = "Folder 1"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(116, 22)
        Me.ToolStripMenuItem4.Text = "Folder 2"
        '
        'tsmiGallery
        '
        Me.tsmiGallery.Name = "tsmiGallery"
        Me.tsmiGallery.Size = New System.Drawing.Size(210, 22)
        Me.tsmiGallery.Text = "ToolStripMenuItem1"
        '
        'tsmiStudio
        '
        Me.tsmiStudio.Name = "tsmiStudio"
        Me.tsmiStudio.Size = New System.Drawing.Size(210, 22)
        Me.tsmiStudio.Text = "ToolStripMenuItem1"
        '
        'tsmiTag
        '
        Me.tsmiTag.Name = "tsmiTag"
        Me.tsmiTag.Size = New System.Drawing.Size(210, 22)
        Me.tsmiTag.Text = "ToolStripMenuItem1"
        '
        'btnOpenTemplate
        '
        Me.btnOpenTemplate.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.btnOpenTemplate.Image = CType(resources.GetObject("btnOpenTemplate.Image"), System.Drawing.Image)
        Me.btnOpenTemplate.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnOpenTemplate.Location = New System.Drawing.Point(10, 18)
        Me.btnOpenTemplate.Name = "btnOpenTemplate"
        Me.btnOpenTemplate.Size = New System.Drawing.Size(66, 60)
        Me.btnOpenTemplate.TabIndex = 21
        Me.btnOpenTemplate.Text = "Open Template"
        Me.btnOpenTemplate.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnOpenTemplate.UseVisualStyleBackColor = True
        '
        'btnSaveAsPoster
        '
        Me.btnSaveAsPoster.Enabled = False
        Me.btnSaveAsPoster.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSaveAsPoster.Image = CType(resources.GetObject("btnSaveAsPoster.Image"), System.Drawing.Image)
        Me.btnSaveAsPoster.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnSaveAsPoster.Location = New System.Drawing.Point(604, 398)
        Me.btnSaveAsPoster.Name = "btnSaveAsPoster"
        Me.btnSaveAsPoster.Size = New System.Drawing.Size(96, 83)
        Me.btnSaveAsPoster.TabIndex = 18
        Me.btnSaveAsPoster.Text = "Save as Poster"
        Me.btnSaveAsPoster.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnSaveAsPoster.UseVisualStyleBackColor = True
        '
        'btnRestorePosition
        '
        Me.btnRestorePosition.Enabled = False
        Me.btnRestorePosition.Image = CType(resources.GetObject("btnRestorePosition.Image"), System.Drawing.Image)
        Me.btnRestorePosition.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRestorePosition.Location = New System.Drawing.Point(365, 428)
        Me.btnRestorePosition.Name = "btnRestorePosition"
        Me.btnRestorePosition.Size = New System.Drawing.Size(23, 23)
        Me.btnRestorePosition.TabIndex = 15
        Me.btnRestorePosition.UseVisualStyleBackColor = True
        '
        'btnRight
        '
        Me.btnRight.Enabled = False
        Me.btnRight.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.right_16x16
        Me.btnRight.Location = New System.Drawing.Point(393, 428)
        Me.btnRight.Name = "btnRight"
        Me.btnRight.Size = New System.Drawing.Size(23, 23)
        Me.btnRight.TabIndex = 14
        Me.btnRight.UseVisualStyleBackColor = True
        '
        'btnLeft
        '
        Me.btnLeft.Enabled = False
        Me.btnLeft.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.left_16x16
        Me.btnLeft.Location = New System.Drawing.Point(337, 428)
        Me.btnLeft.Name = "btnLeft"
        Me.btnLeft.Size = New System.Drawing.Size(23, 23)
        Me.btnLeft.TabIndex = 13
        Me.ttpForm.SetToolTip(Me.btnLeft, "Move base image to left, use Shift for speed up")
        Me.btnLeft.UseVisualStyleBackColor = True
        '
        'btnDown
        '
        Me.btnDown.Enabled = False
        Me.btnDown.Image = CType(resources.GetObject("btnDown.Image"), System.Drawing.Image)
        Me.btnDown.Location = New System.Drawing.Point(365, 455)
        Me.btnDown.Name = "btnDown"
        Me.btnDown.Size = New System.Drawing.Size(23, 23)
        Me.btnDown.TabIndex = 12
        Me.btnDown.UseVisualStyleBackColor = True
        '
        'btnUp
        '
        Me.btnUp.Enabled = False
        Me.btnUp.Image = CType(resources.GetObject("btnUp.Image"), System.Drawing.Image)
        Me.btnUp.Location = New System.Drawing.Point(365, 402)
        Me.btnUp.Name = "btnUp"
        Me.btnUp.Size = New System.Drawing.Size(23, 23)
        Me.btnUp.TabIndex = 11
        Me.btnUp.UseVisualStyleBackColor = True
        '
        'lblPosterSize
        '
        Me.lblPosterSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblPosterSize.Location = New System.Drawing.Point(2, 2)
        Me.lblPosterSize.Name = "lblPosterSize"
        Me.lblPosterSize.Size = New System.Drawing.Size(105, 23)
        Me.lblPosterSize.TabIndex = 5
        Me.lblPosterSize.Text = "Size: (XXXXxXXXX)"
        Me.lblPosterSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblPosterSize.Visible = False
        '
        'btnReloadUntouched
        '
        Me.btnReloadUntouched.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.btnReloadUntouched.Image = Global.generic.EmberCore.PosterEditor.My.Resources.Resources.restore_48x48
        Me.btnReloadUntouched.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnReloadUntouched.Location = New System.Drawing.Point(6, 398)
        Me.btnReloadUntouched.Name = "btnReloadUntouched"
        Me.btnReloadUntouched.Size = New System.Drawing.Size(96, 83)
        Me.btnReloadUntouched.TabIndex = 7
        Me.btnReloadUntouched.Text = "Reload Untouched"
        Me.btnReloadUntouched.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnReloadUntouched.UseVisualStyleBackColor = True
        '
        'pbPoster
        '
        Me.pbPoster.BackColor = System.Drawing.Color.DimGray
        Me.pbPoster.Location = New System.Drawing.Point(0, 0)
        Me.pbPoster.Name = "pbPoster"
        Me.pbPoster.Size = New System.Drawing.Size(700, 394)
        Me.pbPoster.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbPoster.TabIndex = 6
        Me.pbPoster.TabStop = False
        '
        'cmsApplyTemplate
        '
        Me.cmsApplyTemplate.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiApplyToContainingFolder, Me.tsmiApplyToStudio, Me.tsmiApplyToTag})
        Me.cmsApplyTemplate.Name = "cmsSaveTemplate"
        Me.cmsApplyTemplate.Size = New System.Drawing.Size(218, 70)
        '
        'tsmiApplyToContainingFolder
        '
        Me.tsmiApplyToContainingFolder.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem5, Me.ToolStripMenuItem6})
        Me.tsmiApplyToContainingFolder.Name = "tsmiApplyToContainingFolder"
        Me.tsmiApplyToContainingFolder.Size = New System.Drawing.Size(217, 22)
        Me.tsmiApplyToContainingFolder.Text = "Apply to Containing Folder"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(116, 22)
        Me.ToolStripMenuItem5.Text = "Folder 1"
        '
        'ToolStripMenuItem6
        '
        Me.ToolStripMenuItem6.Name = "ToolStripMenuItem6"
        Me.ToolStripMenuItem6.Size = New System.Drawing.Size(116, 22)
        Me.ToolStripMenuItem6.Text = "Folder 2"
        '
        'tsmiApplyToStudio
        '
        Me.tsmiApplyToStudio.Name = "tsmiApplyToStudio"
        Me.tsmiApplyToStudio.Size = New System.Drawing.Size(217, 22)
        Me.tsmiApplyToStudio.Text = "ToolStripMenuItem1"
        '
        'tsmiApplyToTag
        '
        Me.tsmiApplyToTag.Name = "tsmiApplyToTag"
        Me.tsmiApplyToTag.Size = New System.Drawing.Size(217, 22)
        Me.tsmiApplyToTag.Text = "ToolStripMenuItem1"
        '
        'lblBaseImageRotate
        '
        Me.lblBaseImageRotate.AutoSize = True
        Me.lblBaseImageRotate.Location = New System.Drawing.Point(9, 166)
        Me.lblBaseImageRotate.Name = "lblBaseImageRotate"
        Me.lblBaseImageRotate.Size = New System.Drawing.Size(41, 13)
        Me.lblBaseImageRotate.TabIndex = 12
        Me.lblBaseImageRotate.Text = "Rotate"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(264, 188)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(17, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "%"
        '
        'txtBaseImageRotate
        '
        Me.txtBaseImageRotate.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBaseImageRotate.Location = New System.Drawing.Point(217, 185)
        Me.txtBaseImageRotate.Name = "txtBaseImageRotate"
        Me.txtBaseImageRotate.Size = New System.Drawing.Size(46, 22)
        Me.txtBaseImageRotate.TabIndex = 10
        Me.txtBaseImageRotate.Text = "360.00"
        '
        'tbBaseImageRotate
        '
        Me.tbBaseImageRotate.BackColor = System.Drawing.Color.White
        Me.tbBaseImageRotate.Location = New System.Drawing.Point(3, 185)
        Me.tbBaseImageRotate.Maximum = 100
        Me.tbBaseImageRotate.Name = "tbBaseImageRotate"
        Me.tbBaseImageRotate.Size = New System.Drawing.Size(208, 45)
        Me.tbBaseImageRotate.TabIndex = 9
        Me.tbBaseImageRotate.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'frmMoviePosterEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(990, 485)
        Me.Controls.Add(Me.pnlMain)
        Me.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMoviePosterEditor"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmMoviePosterEditor"
        Me.pnlMain.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.tpGeneral.ResumeLayout(False)
        Me.grbCrop.ResumeLayout(False)
        Me.grbCrop.PerformLayout()
        Me.grbOverlay.ResumeLayout(False)
        Me.grbOverlay.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.tpBaseImage.ResumeLayout(False)
        Me.tpBaseImage.PerformLayout()
        CType(Me.tbBaseImageSize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbFrameExtractPercent, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.cmsSaveTemplate.ResumeLayout(False)
        CType(Me.pbPoster, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmsApplyTemplate.ResumeLayout(False)
        CType(Me.tbBaseImageRotate, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents tmrDelay As System.Windows.Forms.Timer
    Friend WithEvents lblPosterSize As Windows.Forms.Label
    Friend WithEvents btnReloadUntouched As Windows.Forms.Button
    Friend WithEvents pbPoster As Windows.Forms.PictureBox
    Friend WithEvents btnDown As Windows.Forms.Button
    Friend WithEvents btnUp As Windows.Forms.Button
    Friend WithEvents grbCrop As Windows.Forms.GroupBox
    Friend WithEvents lblAspectRatio As Windows.Forms.Label
    Friend WithEvents cbAspectRatio As Windows.Forms.ComboBox
    Friend WithEvents cbCropEnabled As Windows.Forms.CheckBox
    Friend WithEvents btnRestorePosition As Windows.Forms.Button
    Friend WithEvents btnRight As Windows.Forms.Button
    Friend WithEvents btnLeft As Windows.Forms.Button
    Friend WithEvents grbOverlay As Windows.Forms.GroupBox
    Friend WithEvents cbOverlayEnabled As Windows.Forms.CheckBox
    Friend WithEvents btnChangeOverlay As Windows.Forms.Button
    Friend WithEvents btnOpenTemplate As Windows.Forms.Button
    Friend WithEvents btnSaveAsPoster As Windows.Forms.Button
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents txtOverlayWidth As Windows.Forms.TextBox
    Friend WithEvents btnSaveTemplate As Windows.Forms.Button
    Friend WithEvents Panel1 As Windows.Forms.Panel
    Friend WithEvents btnActorAdd As Windows.Forms.Button
    Friend WithEvents cbPreviewCropping As Windows.Forms.CheckBox
    Friend WithEvents cbShowCropBorder As Windows.Forms.CheckBox
    Friend WithEvents rbVAlignTop As Windows.Forms.RadioButton
    Friend WithEvents rbVAlignStretch As Windows.Forms.RadioButton
    Friend WithEvents rbVAlignBottom As Windows.Forms.RadioButton
    Friend WithEvents rbVAlignMiddle As Windows.Forms.RadioButton
    Friend WithEvents Panel2 As Windows.Forms.Panel
    Friend WithEvents rbAlignStretch As Windows.Forms.RadioButton
    Friend WithEvents rbAlignRight As Windows.Forms.RadioButton
    Friend WithEvents rbAlignCenter As Windows.Forms.RadioButton
    Friend WithEvents rbAlignLeft As Windows.Forms.RadioButton
    Friend WithEvents lblOverlayWidth As Windows.Forms.Label
    Friend WithEvents cmsSaveTemplate As Windows.Forms.ContextMenuStrip
    Friend WithEvents tsmiMovie As Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmiContainingFolder As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmiGallery As Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmiStudio As Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmiTag As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ofdLocalFiles As Windows.Forms.OpenFileDialog
    Friend WithEvents TabControl1 As Windows.Forms.TabControl
    Friend WithEvents tpGeneral As Windows.Forms.TabPage
    Friend WithEvents tpBaseImage As Windows.Forms.TabPage
    Friend WithEvents tbFrameExtractPercent As Windows.Forms.TrackBar
    Friend WithEvents cbExtractFrame As Windows.Forms.CheckBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents txtFrameExtractPercent As Windows.Forms.TextBox
    Friend WithEvents btnApplyTemplate As Windows.Forms.Button
    Friend WithEvents cmsApplyTemplate As Windows.Forms.ContextMenuStrip
    Friend WithEvents tsmiApplyToContainingFolder As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem5 As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem6 As Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmiApplyToStudio As Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmiApplyToTag As Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnOpenImage As Windows.Forms.Button
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents ttpForm As Windows.Forms.ToolTip
    Friend WithEvents lblBaseImageSize As Windows.Forms.Label
    Friend WithEvents lblFramePosition As Windows.Forms.Label
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents txtBaseImageSize As Windows.Forms.TextBox
    Friend WithEvents tbBaseImageSize As Windows.Forms.TrackBar
    Friend WithEvents lblBaseImageRotate As Windows.Forms.Label
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents txtBaseImageRotate As Windows.Forms.TextBox
    Friend WithEvents tbBaseImageRotate As Windows.Forms.TrackBar
End Class
