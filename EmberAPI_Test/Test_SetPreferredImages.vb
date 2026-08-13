' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports EmberAPI

Namespace EmberTests

    <TestClass()>
    Public Class Test_SetPreferredImages

        Private _previousPosterFrodo As Boolean
        Private _previousKeepExisting As Boolean

        <TestInitialize>
        Public Sub TestInit()
            _previousPosterFrodo = Master.eSettings.MoviePosterFrodo
            _previousKeepExisting = Master.eSettings.MoviePosterKeepExisting
            Master.eSettings.MoviePosterFrodo = True
            Master.eSettings.MoviePosterKeepExisting = False
        End Sub

        <TestCleanup>
        Public Sub TestCleanup()
            Master.eSettings.MoviePosterFrodo = _previousPosterFrodo
            Master.eSettings.MoviePosterKeepExisting = _previousKeepExisting
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub SetPreferredImages_EmptySearchResults_KeepsExistingPoster()
            Const existingPath As String = "C:\library\movie-poster.jpg"
            Dim db As New Database.DBElement(Enums.ContentType.Movie)
            db.Movie = New MediaContainers.Movie()
            db.ImagesContainer.Poster.LocalFilePath = existingPath

            Dim emptyResults As New MediaContainers.SearchResultsContainer()
            Dim modifiers As New Structures.ScrapeModifiers With {.MainPoster = True}

            Images.SetPreferredImages(db, emptyResults, modifiers, IsAutoScraper:=True)

            Assert.AreEqual(existingPath, db.ImagesContainer.Poster.LocalFilePath)
        End Sub

    End Class

End Namespace
