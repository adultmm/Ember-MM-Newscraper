' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports System.Drawing
Imports System.Windows.Forms

Public Module ThreeButtonPromptHelper

    Public Enum ThreeButtonChoice
        None = 0
        First = 1
        Second = 2
        Third = 3
    End Enum

    ''' <summary>
    ''' Simple three-button prompt with custom button labels (MessageBox cannot do this).
    ''' </summary>
    Public Function Show(ByVal title As String,
                         ByVal message As String,
                         ByVal firstButtonText As String,
                         ByVal secondButtonText As String,
                         ByVal thirdButtonText As String,
                         Optional ByVal owner As IWin32Window = Nothing) As ThreeButtonChoice

        Using dlg As New Form()
            dlg.Text = title
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog
            dlg.StartPosition = FormStartPosition.CenterParent
            dlg.MinimizeBox = False
            dlg.MaximizeBox = False
            dlg.ShowInTaskbar = False
            dlg.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
            dlg.ClientSize = New Size(500, 170)

            Dim lblMessage As New Label With {
                .Text = message,
                .AutoSize = False,
                .Location = New Point(12, 12),
                .Size = New Size(476, 110),
                .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
            }

            Dim btnFirst As New Button With {.Text = firstButtonText, .AutoSize = True, .MinimumSize = New Size(75, 23)}
            Dim btnSecond As New Button With {.Text = secondButtonText, .AutoSize = True, .MinimumSize = New Size(75, 23)}
            Dim btnThird As New Button With {.Text = thirdButtonText, .AutoSize = True, .MinimumSize = New Size(75, 23)}

            Dim pnlButtons As New FlowLayoutPanel With {
                .FlowDirection = FlowDirection.RightToLeft,
                .Dock = DockStyle.Bottom,
                .AutoSize = True,
                .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                .Padding = New Padding(0, 8, 12, 12),
                .WrapContents = False
            }
            pnlButtons.Controls.Add(btnThird)
            pnlButtons.Controls.Add(btnSecond)
            pnlButtons.Controls.Add(btnFirst)

            ' Escape / Cancel maps to the second button ("Not now" / "Continue"), not "Don't ask again".
            dlg.CancelButton = btnSecond
            dlg.Controls.Add(lblMessage)
            dlg.Controls.Add(pnlButtons)

            Dim result As ThreeButtonChoice = ThreeButtonChoice.None
            AddHandler btnFirst.Click, Sub(s, e)
                                           result = ThreeButtonChoice.First
                                           dlg.DialogResult = DialogResult.OK
                                       End Sub
            AddHandler btnSecond.Click, Sub(s, e)
                                            result = ThreeButtonChoice.Second
                                            dlg.DialogResult = DialogResult.Cancel
                                        End Sub
            AddHandler btnThird.Click, Sub(s, e)
                                           result = ThreeButtonChoice.Third
                                           dlg.DialogResult = DialogResult.OK
                                       End Sub

            If owner IsNot Nothing Then
                dlg.ShowDialog(owner)
            ElseIf Application.OpenForms.Count > 0 Then
                dlg.ShowDialog(Application.OpenForms(0))
            Else
                dlg.ShowDialog()
            End If

            Return result
        End Using
    End Function

End Module
