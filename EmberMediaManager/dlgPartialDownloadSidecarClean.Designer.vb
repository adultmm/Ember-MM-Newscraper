<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class dlgPartialDownloadSidecarClean
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.tblMain = New System.Windows.Forms.TableLayoutPanel()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblPath = New System.Windows.Forms.Label()
        Me.lstSidecars = New System.Windows.Forms.ListBox()
        Me.chkApplyToAll = New System.Windows.Forms.CheckBox()
        Me.flpButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnDeleteFiles = New System.Windows.Forms.Button()
        Me.btnKeepFiles = New System.Windows.Forms.Button()
        Me.btnAbort = New System.Windows.Forms.Button()
        Me.tblMain.SuspendLayout()
        Me.flpButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'tblMain
        '
        Me.tblMain.ColumnCount = 1
        Me.tblMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tblMain.Controls.Add(Me.lblTitle, 0, 0)
        Me.tblMain.Controls.Add(Me.lblPath, 0, 1)
        Me.tblMain.Controls.Add(Me.lstSidecars, 0, 2)
        Me.tblMain.Controls.Add(Me.chkApplyToAll, 0, 3)
        Me.tblMain.Controls.Add(Me.flpButtons, 0, 4)
        Me.tblMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tblMain.Location = New System.Drawing.Point(0, 0)
        Me.tblMain.Name = "tblMain"
        Me.tblMain.Padding = New System.Windows.Forms.Padding(12)
        Me.tblMain.RowCount = 5
        Me.tblMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tblMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tblMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tblMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tblMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tblMain.Size = New System.Drawing.Size(584, 361)
        Me.tblMain.TabIndex = 0
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.Location = New System.Drawing.Point(15, 12)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(3, 0, 3, 6)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(554, 15)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Title"
        '
        'lblPath
        '
        Me.lblPath.AutoSize = True
        Me.lblPath.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblPath.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.lblPath.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblPath.Location = New System.Drawing.Point(15, 33)
        Me.lblPath.Margin = New System.Windows.Forms.Padding(3, 0, 3, 8)
        Me.lblPath.Name = "lblPath"
        Me.lblPath.Size = New System.Drawing.Size(554, 13)
        Me.lblPath.TabIndex = 1
        Me.lblPath.Text = "Path"
        '
        'lstSidecars
        '
        Me.lstSidecars.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstSidecars.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.lstSidecars.FormattingEnabled = True
        Me.lstSidecars.HorizontalScrollbar = True
        Me.lstSidecars.IntegralHeight = False
        Me.lstSidecars.Location = New System.Drawing.Point(15, 54)
        Me.lstSidecars.Margin = New System.Windows.Forms.Padding(3, 0, 3, 8)
        Me.lstSidecars.Name = "lstSidecars"
        Me.lstSidecars.SelectionMode = System.Windows.Forms.SelectionMode.None
        Me.lstSidecars.Size = New System.Drawing.Size(554, 230)
        Me.lstSidecars.TabIndex = 2
        '
        'chkApplyToAll
        '
        Me.chkApplyToAll.AutoSize = True
        Me.chkApplyToAll.Dock = System.Windows.Forms.DockStyle.Fill
        Me.chkApplyToAll.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.chkApplyToAll.Location = New System.Drawing.Point(15, 295)
        Me.chkApplyToAll.Margin = New System.Windows.Forms.Padding(3, 3, 3, 8)
        Me.chkApplyToAll.Name = "chkApplyToAll"
        Me.chkApplyToAll.Size = New System.Drawing.Size(554, 17)
        Me.chkApplyToAll.TabIndex = 3
        Me.chkApplyToAll.Text = "Apply this choice to all remaining entries in this clean"
        Me.chkApplyToAll.UseVisualStyleBackColor = True
        '
        'flpButtons
        '
        Me.flpButtons.AutoSize = True
        Me.flpButtons.Controls.Add(Me.btnDeleteFiles)
        Me.flpButtons.Controls.Add(Me.btnKeepFiles)
        Me.flpButtons.Controls.Add(Me.btnAbort)
        Me.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.flpButtons.Location = New System.Drawing.Point(15, 323)
        Me.flpButtons.Name = "flpButtons"
        Me.flpButtons.Size = New System.Drawing.Size(554, 29)
        Me.flpButtons.TabIndex = 4
        Me.flpButtons.WrapContents = False
        '
        'btnDeleteFiles
        '
        Me.btnDeleteFiles.AutoSize = True
        Me.btnDeleteFiles.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.btnDeleteFiles.Location = New System.Drawing.Point(452, 3)
        Me.btnDeleteFiles.Name = "btnDeleteFiles"
        Me.btnDeleteFiles.Size = New System.Drawing.Size(99, 23)
        Me.btnDeleteFiles.TabIndex = 0
        Me.btnDeleteFiles.Text = "Delete files"
        Me.btnDeleteFiles.UseVisualStyleBackColor = True
        '
        'btnKeepFiles
        '
        Me.btnKeepFiles.AutoSize = True
        Me.btnKeepFiles.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.btnKeepFiles.Location = New System.Drawing.Point(367, 3)
        Me.btnKeepFiles.Name = "btnKeepFiles"
        Me.btnKeepFiles.Size = New System.Drawing.Size(79, 23)
        Me.btnKeepFiles.TabIndex = 1
        Me.btnKeepFiles.Text = "Keep files"
        Me.btnKeepFiles.UseVisualStyleBackColor = True
        '
        'btnAbort
        '
        Me.btnAbort.AutoSize = True
        Me.btnAbort.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.btnAbort.Location = New System.Drawing.Point(306, 3)
        Me.btnAbort.Name = "btnAbort"
        Me.btnAbort.Size = New System.Drawing.Size(55, 23)
        Me.btnAbort.TabIndex = 2
        Me.btnAbort.Text = "Abort"
        Me.btnAbort.UseVisualStyleBackColor = True
        '
        'dlgPartialDownloadSidecarClean
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(584, 361)
        Me.Controls.Add(Me.tblMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgPartialDownloadSidecarClean"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Partial download sidecar files"
        Me.tblMain.ResumeLayout(False)
        Me.tblMain.PerformLayout()
        Me.flpButtons.ResumeLayout(False)
        Me.flpButtons.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tblMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblPath As System.Windows.Forms.Label
    Friend WithEvents lstSidecars As System.Windows.Forms.ListBox
    Friend WithEvents chkApplyToAll As System.Windows.Forms.CheckBox
    Friend WithEvents flpButtons As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnDeleteFiles As System.Windows.Forms.Button
    Friend WithEvents btnKeepFiles As System.Windows.Forms.Button
    Friend WithEvents btnAbort As System.Windows.Forms.Button

End Class
