<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class dlgApplyTemplate
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
        Me.lblMessage = New System.Windows.Forms.Label()
        Me.pbProgress = New System.Windows.Forms.ProgressBar()
        Me.bwPosterGenerator = New System.ComponentModel.BackgroundWorker()
        Me.pnlSettings = New System.Windows.Forms.Panel()
        Me.rbAll = New System.Windows.Forms.RadioButton()
        Me.rbUntouchedBaseImage = New System.Windows.Forms.RadioButton()
        Me.rbUntouchedPoster = New System.Windows.Forms.RadioButton()
        Me.rbWithoutPoster = New System.Windows.Forms.RadioButton()
        Me.pnlProgress = New System.Windows.Forms.Panel()
        Me.lblSkippedDetails = New System.Windows.Forms.Label()
        Me.lblErrorValue = New System.Windows.Forms.Label()
        Me.lblError = New System.Windows.Forms.Label()
        Me.lblSkippedValue = New System.Windows.Forms.Label()
        Me.lblSkipped = New System.Windows.Forms.Label()
        Me.lblReadyValue = New System.Windows.Forms.Label()
        Me.lblReady = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnNo = New System.Windows.Forms.Button()
        Me.btnYes = New System.Windows.Forms.Button()
        Me.pnlSettings.SuspendLayout()
        Me.pnlProgress.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblMessage
        '
        Me.lblMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.lblMessage.Location = New System.Drawing.Point(13, 13)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(410, 42)
        Me.lblMessage.TabIndex = 5
        Me.lblMessage.Text = "Do you want to apply the template and regenerate posters of movies with Alpha Stu" &
    "dio..."
        '
        'pbProgress
        '
        Me.pbProgress.Location = New System.Drawing.Point(16, 220)
        Me.pbProgress.Name = "pbProgress"
        Me.pbProgress.Size = New System.Drawing.Size(407, 23)
        Me.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.pbProgress.TabIndex = 6
        '
        'bwPosterGenerator
        '
        Me.bwPosterGenerator.WorkerReportsProgress = True
        Me.bwPosterGenerator.WorkerSupportsCancellation = True
        '
        'pnlSettings
        '
        Me.pnlSettings.Controls.Add(Me.rbAll)
        Me.pnlSettings.Controls.Add(Me.rbUntouchedBaseImage)
        Me.pnlSettings.Controls.Add(Me.rbUntouchedPoster)
        Me.pnlSettings.Controls.Add(Me.rbWithoutPoster)
        Me.pnlSettings.Location = New System.Drawing.Point(16, 54)
        Me.pnlSettings.Name = "pnlSettings"
        Me.pnlSettings.Size = New System.Drawing.Size(407, 160)
        Me.pnlSettings.TabIndex = 7
        '
        'rbAll
        '
        Me.rbAll.AutoSize = True
        Me.rbAll.Location = New System.Drawing.Point(20, 122)
        Me.rbAll.Name = "rbAll"
        Me.rbAll.Size = New System.Drawing.Size(269, 17)
        Me.rbAll.TabIndex = 8
        Me.rbAll.Text = "... all Movies, regenerate base image if not available"
        Me.rbAll.UseVisualStyleBackColor = True
        '
        'rbUntouchedBaseImage
        '
        Me.rbUntouchedBaseImage.AutoSize = True
        Me.rbUntouchedBaseImage.Location = New System.Drawing.Point(21, 86)
        Me.rbUntouchedBaseImage.Name = "rbUntouchedBaseImage"
        Me.rbUntouchedBaseImage.Size = New System.Drawing.Size(268, 17)
        Me.rbUntouchedBaseImage.TabIndex = 7
        Me.rbUntouchedBaseImage.Text = "...and where the untouched base image is available"
        Me.rbUntouchedBaseImage.UseVisualStyleBackColor = True
        '
        'rbUntouchedPoster
        '
        Me.rbUntouchedPoster.AutoSize = True
        Me.rbUntouchedPoster.Location = New System.Drawing.Point(21, 51)
        Me.rbUntouchedPoster.Name = "rbUntouchedPoster"
        Me.rbUntouchedPoster.Size = New System.Drawing.Size(261, 17)
        Me.rbUntouchedPoster.TabIndex = 6
        Me.rbUntouchedPoster.Text = "...and with untouched poster (no template applied)"
        Me.rbUntouchedPoster.UseVisualStyleBackColor = True
        '
        'rbWithoutPoster
        '
        Me.rbWithoutPoster.AutoSize = True
        Me.rbWithoutPoster.Checked = True
        Me.rbWithoutPoster.Location = New System.Drawing.Point(21, 16)
        Me.rbWithoutPoster.Name = "rbWithoutPoster"
        Me.rbWithoutPoster.Size = New System.Drawing.Size(122, 17)
        Me.rbWithoutPoster.TabIndex = 5
        Me.rbWithoutPoster.TabStop = True
        Me.rbWithoutPoster.Text = "...and without Poster"
        Me.rbWithoutPoster.UseVisualStyleBackColor = True
        '
        'pnlProgress
        '
        Me.pnlProgress.Controls.Add(Me.lblSkippedDetails)
        Me.pnlProgress.Controls.Add(Me.lblErrorValue)
        Me.pnlProgress.Controls.Add(Me.lblError)
        Me.pnlProgress.Controls.Add(Me.lblSkippedValue)
        Me.pnlProgress.Controls.Add(Me.lblSkipped)
        Me.pnlProgress.Controls.Add(Me.lblReadyValue)
        Me.pnlProgress.Controls.Add(Me.lblReady)
        Me.pnlProgress.Location = New System.Drawing.Point(291, 54)
        Me.pnlProgress.Name = "pnlProgress"
        Me.pnlProgress.Size = New System.Drawing.Size(132, 160)
        Me.pnlProgress.TabIndex = 9
        Me.pnlProgress.Visible = False
        '
        'lblSkippedDetails
        '
        Me.lblSkippedDetails.AutoSize = True
        Me.lblSkippedDetails.Location = New System.Drawing.Point(9, 70)
        Me.lblSkippedDetails.Name = "lblSkippedDetails"
        Me.lblSkippedDetails.Size = New System.Drawing.Size(107, 13)
        Me.lblSkippedDetails.TabIndex = 6
        Me.lblSkippedDetails.Text = "Because no template"
        '
        'lblErrorValue
        '
        Me.lblErrorValue.AutoSize = True
        Me.lblErrorValue.Location = New System.Drawing.Point(62, 106)
        Me.lblErrorValue.Name = "lblErrorValue"
        Me.lblErrorValue.Size = New System.Drawing.Size(31, 13)
        Me.lblErrorValue.TabIndex = 5
        Me.lblErrorValue.Text = "8888"
        '
        'lblError
        '
        Me.lblError.AutoSize = True
        Me.lblError.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.lblError.Location = New System.Drawing.Point(9, 106)
        Me.lblError.Name = "lblError"
        Me.lblError.Size = New System.Drawing.Size(38, 13)
        Me.lblError.TabIndex = 4
        Me.lblError.Text = "Error:"
        '
        'lblSkippedValue
        '
        Me.lblSkippedValue.AutoSize = True
        Me.lblSkippedValue.Location = New System.Drawing.Point(62, 51)
        Me.lblSkippedValue.Name = "lblSkippedValue"
        Me.lblSkippedValue.Size = New System.Drawing.Size(31, 13)
        Me.lblSkippedValue.TabIndex = 3
        Me.lblSkippedValue.Text = "8888"
        '
        'lblSkipped
        '
        Me.lblSkipped.AutoSize = True
        Me.lblSkipped.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.lblSkipped.Location = New System.Drawing.Point(9, 51)
        Me.lblSkipped.Name = "lblSkipped"
        Me.lblSkipped.Size = New System.Drawing.Size(57, 13)
        Me.lblSkipped.TabIndex = 2
        Me.lblSkipped.Text = "Skipped:"
        '
        'lblReadyValue
        '
        Me.lblReadyValue.AutoSize = True
        Me.lblReadyValue.Location = New System.Drawing.Point(62, 16)
        Me.lblReadyValue.Name = "lblReadyValue"
        Me.lblReadyValue.Size = New System.Drawing.Size(31, 13)
        Me.lblReadyValue.TabIndex = 1
        Me.lblReadyValue.Text = "8888"
        '
        'lblReady
        '
        Me.lblReady.AutoSize = True
        Me.lblReady.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.lblReady.Location = New System.Drawing.Point(9, 16)
        Me.lblReady.Name = "lblReady"
        Me.lblReady.Size = New System.Drawing.Size(47, 13)
        Me.lblReady.TabIndex = 0
        Me.lblReady.Text = "Ready:"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.btnCancel)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnOK)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnNo)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnYes)
        Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(131, 274)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(292, 29)
        Me.FlowLayoutPanel1.TabIndex = 10
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnCancel.Location = New System.Drawing.Point(222, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(67, 23)
        Me.btnCancel.TabIndex = 7
        Me.btnCancel.Text = "Cancel"
        '
        'btnOK
        '
        Me.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnOK.Location = New System.Drawing.Point(149, 3)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(67, 23)
        Me.btnOK.TabIndex = 6
        Me.btnOK.Text = "OK"
        '
        'btnNo
        '
        Me.btnNo.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnNo.Location = New System.Drawing.Point(76, 3)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(67, 23)
        Me.btnNo.TabIndex = 5
        Me.btnNo.Text = "No"
        '
        'btnYes
        '
        Me.btnYes.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnYes.Location = New System.Drawing.Point(3, 3)
        Me.btnYes.Name = "btnYes"
        Me.btnYes.Size = New System.Drawing.Size(67, 23)
        Me.btnYes.TabIndex = 4
        Me.btnYes.Text = "Yes"
        '
        'dlgApplyTemplate
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(435, 315)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.pnlProgress)
        Me.Controls.Add(Me.pnlSettings)
        Me.Controls.Add(Me.pbProgress)
        Me.Controls.Add(Me.lblMessage)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgApplyTemplate"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "dlgApplyTemplate"
        Me.pnlSettings.ResumeLayout(False)
        Me.pnlSettings.PerformLayout()
        Me.pnlProgress.ResumeLayout(False)
        Me.pnlProgress.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblMessage As Windows.Forms.Label
    Friend WithEvents pbProgress As Windows.Forms.ProgressBar
    Friend WithEvents bwPosterGenerator As ComponentModel.BackgroundWorker
    Friend WithEvents pnlSettings As Windows.Forms.Panel
    Friend WithEvents rbAll As Windows.Forms.RadioButton
    Friend WithEvents rbUntouchedBaseImage As Windows.Forms.RadioButton
    Friend WithEvents rbUntouchedPoster As Windows.Forms.RadioButton
    Friend WithEvents rbWithoutPoster As Windows.Forms.RadioButton
    Friend WithEvents pnlProgress As Windows.Forms.Panel
    Friend WithEvents lblErrorValue As Windows.Forms.Label
    Friend WithEvents lblError As Windows.Forms.Label
    Friend WithEvents lblSkippedValue As Windows.Forms.Label
    Friend WithEvents lblSkipped As Windows.Forms.Label
    Friend WithEvents lblReadyValue As Windows.Forms.Label
    Friend WithEvents lblReady As Windows.Forms.Label
    Friend WithEvents lblSkippedDetails As Windows.Forms.Label
    Friend WithEvents FlowLayoutPanel1 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnCancel As Windows.Forms.Button
    Friend WithEvents btnOK As Windows.Forms.Button
    Friend WithEvents btnNo As Windows.Forms.Button
    Friend WithEvents btnYes As Windows.Forms.Button
End Class
