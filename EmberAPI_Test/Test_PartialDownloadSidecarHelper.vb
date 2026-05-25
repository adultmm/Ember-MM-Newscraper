' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports EmberAPI

Namespace EmberTests

    <TestClass()>
    Public Class Test_PartialDownloadSidecarHelper

        Private _tempDir As String

        <TestInitialize()>
        Public Sub TestInitialize()
            _tempDir = Path.Combine(Path.GetTempPath(), "EmberPartialSidecarTest_" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(_tempDir)
        End Sub

        <TestCleanup()>
        Public Sub TestCleanup()
            If Directory.Exists(_tempDir) Then
                Directory.Delete(_tempDir, True)
            End If
        End Sub

        Private Function TouchFile(ByVal fileName As String) As String
            Dim fullPath As String = Path.Combine(_tempDir, fileName)
            File.WriteAllText(fullPath, "test")
            Return fullPath
        End Function

        <UnitTest()>
        <TestMethod()>
        Public Sub FilterSidecarFiles_ExcludesMainVideo()
            Dim mainPath As String = TouchFile("~uTorrentPartFile_ABC.dat")
            Dim items As New List(Of FileSystemInfo) From {
                New FileInfo(mainPath),
                New FileInfo(TouchFile("~uTorrentPartFile_ABC.fanart.jpg")),
                New FileInfo(TouchFile("movie.nfo"))
            }

            Dim sidecars As List(Of String) = PartialDownloadSidecarHelper.FilterSidecarFilePaths(mainPath, items)

            Assert.AreEqual(1, sidecars.Count)
            Assert.IsTrue(sidecars(0).EndsWith("~uTorrentPartFile_ABC.fanart.jpg", StringComparison.OrdinalIgnoreCase))
            Assert.IsFalse(sidecars.Contains(mainPath, StringComparer.OrdinalIgnoreCase))
            Assert.IsFalse(sidecars.Any(Function(p) p.EndsWith("movie.nfo", StringComparison.OrdinalIgnoreCase)))
        End Sub

        <UnitTest()>
        <TestMethod()>
        Public Sub FilterSidecarFiles_MainVideoOnly_ReturnsEmpty()
            Dim mainPath As String = TouchFile("~uTorrentPartFile_ABC.dat")
            Dim items As New List(Of FileSystemInfo) From {
                New FileInfo(mainPath)
            }

            Dim sidecars As List(Of String) = PartialDownloadSidecarHelper.FilterSidecarFilePaths(mainPath, items)

            Assert.AreEqual(0, sidecars.Count)
        End Sub

        <UnitTest()>
        <TestMethod()>
        Public Sub FilterSidecarFiles_CaseInsensitiveMainPathMatch()
            Dim mainPath As String = TouchFile("Movie.mkv")
            Dim items As New List(Of FileSystemInfo) From {
                New FileInfo(Path.Combine(_tempDir, "MOVIE.MKV")),
                New FileInfo(TouchFile("movie-fanart.jpg"))
            }

            Dim sidecars As List(Of String) = PartialDownloadSidecarHelper.FilterSidecarFilePaths(mainPath, items)

            Assert.AreEqual(1, sidecars.Count)
            Assert.IsTrue(sidecars(0).EndsWith("movie-fanart.jpg", StringComparison.OrdinalIgnoreCase))
        End Sub

        <UnitTest()>
        <TestMethod()>
        Public Sub FilterSidecarFiles_SkipsNonExistingFiles()
            Dim mainPath As String = TouchFile("Movie.mkv")
            Dim items As New List(Of FileSystemInfo) From {
                New FileInfo(mainPath),
                New FileInfo(Path.Combine(_tempDir, "missing-fanart.jpg"))
            }

            Dim sidecars As List(Of String) = PartialDownloadSidecarHelper.FilterSidecarFilePaths(mainPath, items)

            Assert.AreEqual(0, sidecars.Count)
        End Sub

        <UnitTest()>
        <TestMethod()>
        Public Sub GetExistingSidecarFiles_IsSingleMovieFolder_IncludesSiblingSidecars()
            Dim mainPath As String = TouchFile("~uTorrentPartFile_ABC.dat")
            Dim fanartPath As String = TouchFile("~uTorrentPartFile_ABC.fanart.jpg")
            Dim dbElement As New Database.DBElement(Enums.ContentType.Movie) With {
                .Filename = mainPath,
                .IsSingle = True
            }

            Dim sidecars As List(Of String) = PartialDownloadSidecarHelper.GetExistingSidecarFiles(dbElement)

            Assert.AreEqual(1, sidecars.Count)
            Assert.IsTrue(String.Equals(fanartPath, sidecars(0), StringComparison.OrdinalIgnoreCase))
        End Sub

        <UnitTest()>
        <TestMethod()>
        Public Sub IsPartialSidecarFile_OnlyMatchesSameBaseNamePrefix()
            Dim mainPath As String = "P:\Movies\~uTorrentPartFile_ABC.dat"

            Assert.IsTrue(PartialDownloadSidecarHelper.IsPartialSidecarFile(mainPath, "P:\Movies\~uTorrentPartFile_ABC.fanart.jpg"))
            Assert.IsFalse(PartialDownloadSidecarHelper.IsPartialSidecarFile(mainPath, "P:\Movies\fanart.jpg"))
            Assert.IsFalse(PartialDownloadSidecarHelper.IsPartialSidecarFile(mainPath, "P:\Movies\Girls Gone Wild-fanart.jpg"))
        End Sub

        <UnitTest()>
        <TestMethod()>
        Public Sub GetExistingSidecarFiles_ExcludesUnrelatedFilesInSameFolder()
            Dim mainPath As String = TouchFile("~uTorrentPartFile_ABC.dat")
            TouchFile("~uTorrentPartFile_ABC.fanart.jpg")
            TouchFile("Girls Gone Wild-fanart.jpg")
            TouchFile("poster.jpg")
            Dim dbElement As New Database.DBElement(Enums.ContentType.Movie) With {
                .Filename = mainPath,
                .IsSingle = True
            }

            Dim sidecars As List(Of String) = PartialDownloadSidecarHelper.GetExistingSidecarFiles(dbElement)

            Assert.AreEqual(1, sidecars.Count)
            Assert.IsTrue(sidecars(0).EndsWith("~uTorrentPartFile_ABC.fanart.jpg", StringComparison.OrdinalIgnoreCase))
        End Sub

    End Class

End Namespace
