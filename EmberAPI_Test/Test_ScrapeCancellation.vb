' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports EmberAPI

Namespace EmberTests

    <TestClass()>
    Public Class Test_ScrapeCancellation

        <TestCleanup>
        Public Sub TestCleanup()
            ScrapeCancellation.Reset()
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub Default_IsNotRequested()
            ScrapeCancellation.Reset()
            Assert.IsFalse(ScrapeCancellation.IsRequested)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub Request_SetsIsRequested_Issue173()
            ScrapeCancellation.Reset()
            ScrapeCancellation.Request()
            Assert.IsTrue(ScrapeCancellation.IsRequested)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub Reset_ClearsIsRequested_Issue173()
            ScrapeCancellation.Request()
            ScrapeCancellation.Reset()
            Assert.IsFalse(ScrapeCancellation.IsRequested)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub Request_Twice_IsIdempotent()
            ScrapeCancellation.Reset()
            ScrapeCancellation.Request()
            ScrapeCancellation.Request()
            Assert.IsTrue(ScrapeCancellation.IsRequested)
        End Sub

    End Class

End Namespace
