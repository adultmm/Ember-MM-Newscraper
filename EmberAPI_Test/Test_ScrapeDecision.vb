Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports EmberAPI

Namespace EmberTests

    <TestClass()>
    Public Class Test_ScrapeDecision

        Private Shared Function MainNfoModifiers() As Structures.ScrapeModifiers
            Return New Structures.ScrapeModifiers With {.MainNFO = True}
        End Function

        <UnitTest>
        <TestMethod()>
        Public Sub ShouldSkip_Cancelled_ReturnsTrue()
            Assert.IsTrue(ModulesManager.ShouldSkipMovieScrapeItem(True, True, MainNfoModifiers()))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ShouldSkip_NoResults_MainNfo_ReturnsTrue_Issue160()
            Assert.IsTrue(ModulesManager.ShouldSkipMovieScrapeItem(False, False, MainNfoModifiers()))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ShouldSkip_ResultWithoutImdbOrTmdb_NotSkipped_Issue170()
            Assert.IsFalse(ModulesManager.ShouldSkipMovieScrapeItem(True, False, MainNfoModifiers()))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ShouldSkip_ResultWithMainNfoOff_NotSkipped()
            Dim modifiers As New Structures.ScrapeModifiers With {.MainNFO = False}
            Assert.IsFalse(ModulesManager.ShouldSkipMovieScrapeItem(False, False, modifiers))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ShouldSkip_CancelledOverridesResults_ReturnsTrue()
            Assert.IsTrue(ModulesManager.ShouldSkipMovieScrapeItem(True, True, MainNfoModifiers()))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub IsSingleItemScrapeRun_SingleField_Multi_ReturnsFalse_Issue159()
            Assert.IsFalse(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.SingleField, 3))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub IsSingleItemScrapeRun_SingleField_One_ReturnsTrue()
            Assert.IsTrue(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.SingleField, 1))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub IsSingleItemScrapeRun_SingleField_Zero_ReturnsTrue()
            Assert.IsTrue(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.SingleField, 0))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub IsSingleItemScrapeRun_SingleScrape_One_ReturnsTrue()
            Assert.IsTrue(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.SingleScrape, 1))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub IsSingleItemScrapeRun_SingleScrape_Multi_ReturnsTrue()
            Assert.IsTrue(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.SingleScrape, 5))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub IsSingleItemScrapeRun_SingleAuto_One_ReturnsTrue()
            Assert.IsTrue(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.SingleAuto, 1))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub IsSingleItemScrapeRun_SelectedAsk_Multi_ReturnsFalse()
            Assert.IsFalse(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.SelectedAsk, 5))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub IsSingleItemScrapeRun_SelectedAsk_One_ReturnsFalse()
            Assert.IsFalse(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.SelectedAsk, 1))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub IsSingleItemScrapeRun_BatchTypes_ReturnFalse()
            Assert.IsFalse(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.AllAsk, 10))
            Assert.IsFalse(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.NewAuto, 2))
            Assert.IsFalse(ModulesManager.IsSingleItemScrapeRun(Enums.ScrapeType.MarkedSkip, 4))
        End Sub

    End Class

End Namespace
