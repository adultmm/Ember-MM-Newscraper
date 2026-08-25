Imports System.Windows.Forms
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports EmberAPI

Namespace EmberTests

    <TestClass()>
    Public Class Test_EditDialogDecision

        <UnitTest>
        <TestMethod()>
        Public Sub ShouldStartChangeMediaScrape_UserChangeClick_ReturnsTrue_Issue173()
            Assert.IsTrue(EditDialogDecision.ShouldStartChangeMediaScrape(
                DialogResult.Abort, changeMediaRequested:=True, scrapeCancellationRequested:=False))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ShouldStartChangeMediaScrape_HostCancelAbort_ReturnsFalse_Issue173()
            Assert.IsFalse(EditDialogDecision.ShouldStartChangeMediaScrape(
                DialogResult.Abort, changeMediaRequested:=False, scrapeCancellationRequested:=False))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ShouldStartChangeMediaScrape_StickyCancelWithChangeFlag_ReturnsFalse_Issue173()
            Assert.IsFalse(EditDialogDecision.ShouldStartChangeMediaScrape(
                DialogResult.Abort, changeMediaRequested:=True, scrapeCancellationRequested:=True))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ShouldStartChangeMediaScrape_CancelResult_ReturnsFalse_Issue173()
            Assert.IsFalse(EditDialogDecision.ShouldStartChangeMediaScrape(
                DialogResult.Cancel, changeMediaRequested:=True, scrapeCancellationRequested:=False))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ShouldStartChangeMediaScrape_OkResult_ReturnsFalse_Issue173()
            Assert.IsFalse(EditDialogDecision.ShouldStartChangeMediaScrape(
                DialogResult.OK, changeMediaRequested:=False, scrapeCancellationRequested:=False))
        End Sub

    End Class

End Namespace
