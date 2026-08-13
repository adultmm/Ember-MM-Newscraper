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

    End Class

End Namespace
