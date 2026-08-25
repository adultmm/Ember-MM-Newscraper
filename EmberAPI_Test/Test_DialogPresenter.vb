' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports System.Windows.Forms
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports EmberAPI

Namespace EmberTests

    <TestClass()>
    Public Class Test_DialogPresenter

        <TestCleanup>
        Public Sub TestCleanup()
            ScrapeCancellation.Reset()
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub CancelActive_WithoutActiveDialog_DoesNotThrow()
            DialogPresenter.CancelActive()
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub HasActiveDialog_WithoutDialog_ReturnsFalse()
            Assert.IsFalse(DialogPresenter.HasActiveDialog)
        End Sub

        <IntegrationTest>
        <TestMethod()>
        Public Sub Present_CancelRequested_ReturnsAbortAndDoesNotShow_Issue173()
            ScrapeCancellation.Request()
            Using f As New Form()
                Dim result = DialogPresenter.Present(f)
                Assert.AreEqual(DialogResult.Abort, result)
                Assert.IsFalse(f.Visible)
                Assert.IsFalse(f.IsHandleCreated)
            End Using
        End Sub

    End Class

End Namespace
