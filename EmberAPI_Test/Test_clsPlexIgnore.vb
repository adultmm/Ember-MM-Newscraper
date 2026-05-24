' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################
' ################################################################################
' # This file is part of Ember Media Manager.                                    #
' #                                                                              #
' # Ember Media Manager is free software: you can redistribute it and/or modify  #
' # it under the terms of the GNU General Public License as published by         #
' # the Free Software Foundation, either version 3 of the License, or            #
' # (at your option) any later version.                                          #
' #                                                                              #
' # Ember Media Manager is distributed in the hope that it will be useful,       #
' # but WITHOUT ANY WARRANTY; without even the implied warranty of               #
' # MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                #
' # GNU General Public License for more details.                                 #
' #                                                                              #
' # You should have received a copy of the GNU General Public License            #
' # along with Ember Media Manager.  If not, see <http://www.gnu.org/licenses/>. #
' ################################################################################

Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports EmberAPI

Namespace EmberTests

    <TestClass()>
    Public Class Test_PlexIgnoreFilter

#Region "Helpers"

        ''' <summary>
        ''' Creates a temporary directory with a .plexignore file containing the given content.
        ''' Returns the directory path. Caller is responsible for cleanup.
        ''' </summary>
        Private Function CreateTempDir(ByVal plexIgnoreContent As String) As String
            Dim dir As String = Path.Combine(Path.GetTempPath(), "PlexIgnoreTest_" & Path.GetRandomFileName())
            Directory.CreateDirectory(dir)
            File.WriteAllText(Path.Combine(dir, ".plexignore"), plexIgnoreContent)
            Return dir
        End Function

        Private Sub CleanupTempDir(ByVal dir As String)
            If Directory.Exists(dir) Then Directory.Delete(dir, True)
        End Sub

#End Region 'Helpers

#Region "No .plexignore file"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_NoFile_NothingIgnored()
            ' Arrange — directory without .plexignore
            Dim dir As String = Path.Combine(Path.GetTempPath(), "PlexIgnoreTest_Empty_" & Path.GetRandomFileName())
            Directory.CreateDirectory(dir)
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                ' Assert
                Assert.IsFalse(filter.IsIgnored("movie.mkv", False))
                Assert.IsFalse(filter.IsIgnored("SomeFolder", True))
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_EmptyPath_NothingIgnored()
            ' Arrange — empty string = no-op filter
            Dim filter As New PlexIgnoreFilter(String.Empty)

            ' Assert
            Assert.IsFalse(filter.IsIgnored("movie.mkv", False))
            Assert.IsFalse(filter.IsIgnored("anything", True))
        End Sub

#End Region

#Region "Comments and empty lines"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_CommentsAndEmptyLines_Ignored()
            ' Arrange
            Dim dir As String = CreateTempDir("# This is a comment" & vbLf & vbLf & "  # another comment  " & vbLf)
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                ' Assert — comments never cause anything to be ignored
                Assert.IsFalse(filter.IsIgnored("movie.mkv", False))
                Assert.IsFalse(filter.IsIgnored("# This is a comment", False))
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

#End Region

#Region "Exact filename match"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_ExactFilename_Ignored()
            Dim dir As String = CreateTempDir("sample.mkv")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("sample.mkv", False), "Exact filename should be ignored")
                Assert.IsFalse(filter.IsIgnored("movie.mkv", False), "Other file should not be ignored")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_ExactFilename_CaseInsensitive()
            Dim dir As String = CreateTempDir("Sample.MKV")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("sample.mkv", False), "Match should be case-insensitive")
                Assert.IsTrue(filter.IsIgnored("SAMPLE.MKV", False), "Match should be case-insensitive")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_IsIgnoredFilePath_MatchesByFullPath()
            Dim dir As String = CreateTempDir("01.mp4" & vbLf & "02.mp4")
            Try
                Assert.IsTrue(PlexIgnoreFilter.IsIgnoredFilePath(Path.Combine(dir, "01.mp4")), "01.mp4 should be ignored by path")
                Assert.IsTrue(PlexIgnoreFilter.IsIgnoredFilePath(Path.Combine(dir, "02.mp4")), "02.mp4 should be ignored by path")
                Assert.IsFalse(PlexIgnoreFilter.IsIgnoredFilePath(Path.Combine(dir, "movie.mkv")), "movie.mkv should not be ignored")
                Assert.IsFalse(PlexIgnoreFilter.IsIgnoredFilePath(String.Empty), "Empty path should not be ignored")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

#End Region

#Region "Wildcard * patterns"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_StarExtension_MatchesAllWithExtension()
            Dim dir As String = CreateTempDir("*.nfo")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("movie.nfo", False), "*.nfo should match movie.nfo")
                Assert.IsTrue(filter.IsIgnored("tvshow.nfo", False), "*.nfo should match tvshow.nfo")
                Assert.IsFalse(filter.IsIgnored("movie.mkv", False), "*.nfo should not match .mkv")
                Assert.IsFalse(filter.IsIgnored("nfo", False), "*.nfo should not match bare 'nfo'")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_StarPrefix_MatchesPartialName()
            Dim dir As String = CreateTempDir("*-trailer.*")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("movie-trailer.mkv", False))
                Assert.IsTrue(filter.IsIgnored("something-trailer.mp4", False))
                Assert.IsFalse(filter.IsIgnored("movie.mkv", False))
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_Star_DoesNotMatchSlash()
            ' * should not cross directory boundaries
            Dim dir As String = CreateTempDir("*.mkv")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("movie.mkv", False))
                ' A path with a slash is not just a name — filter receives names, not paths,
                ' but we verify * doesn't greedily match a separator character
                Assert.IsFalse(filter.IsIgnored("subdir/movie.mkv", False), "* should not match path separator")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

#End Region

#Region "Wildcard ? pattern"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_QuestionMark_MatchesSingleChar()
            Dim dir As String = CreateTempDir("file?.mkv")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("file1.mkv", False))
                Assert.IsTrue(filter.IsIgnored("fileA.mkv", False))
                Assert.IsFalse(filter.IsIgnored("file.mkv", False), "? requires exactly one char")
                Assert.IsFalse(filter.IsIgnored("file12.mkv", False), "? matches only one char")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

#End Region

#Region "Double-star ** pattern"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_DoubleStar_MatchesAcrossDepths()
            Dim dir As String = CreateTempDir("**/sample")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("sample", True), "** should match at root level")
                Assert.IsTrue(filter.IsIgnored("sample", False), "** without trailing / matches files too")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

#End Region

#Region "Directory-only patterns (trailing /)"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_TrailingSlash_OnlyMatchesDirectories()
            Dim dir As String = CreateTempDir("extras/")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("extras", True), "Directory should be ignored")
                Assert.IsFalse(filter.IsIgnored("extras", False), "File with same name should NOT be ignored")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_TrailingSlash_WildcardDirOnly()
            Dim dir As String = CreateTempDir("*sample*/")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("sample", True))
                Assert.IsTrue(filter.IsIgnored("my-sample-dir", True))
                Assert.IsFalse(filter.IsIgnored("sample.mkv", False), "Dir-only pattern must not match files")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

#End Region

#Region "Negation ! patterns"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_Negation_UnignoresFile()
            Dim dir As String = CreateTempDir("*.mkv" & vbLf & "!important.mkv")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("random.mkv", False), "Should be ignored by *.mkv")
                Assert.IsFalse(filter.IsIgnored("important.mkv", False), "Should be un-ignored by negation")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_Negation_OrderMatters_LastRuleWins()
            ' If a negation comes before the ignore rule, the ignore rule wins
            Dim dir As String = CreateTempDir("!important.mkv" & vbLf & "*.mkv")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                ' *.mkv comes after !important.mkv, so *.mkv wins — important.mkv IS ignored
                Assert.IsTrue(filter.IsIgnored("important.mkv", False), "Last matching rule wins")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

#End Region

#Region "Multiple patterns"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_MultiplePatterns_AllApply()
            Dim content As String = String.Join(vbLf, New String() {
                "*.nfo",
                "*.jpg",
                "extras/",
                "sample.*"
            })
            Dim dir As String = CreateTempDir(content)
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("movie.nfo", False))
                Assert.IsTrue(filter.IsIgnored("fanart.jpg", False))
                Assert.IsTrue(filter.IsIgnored("extras", True))
                Assert.IsTrue(filter.IsIgnored("sample.mkv", False))
                Assert.IsFalse(filter.IsIgnored("movie.mkv", False))
                Assert.IsFalse(filter.IsIgnored("extras", False), "extras/ is dir-only")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

#End Region

#Region "Character class [] patterns"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_CharacterClass_MatchesAnyInSet()
            Dim dir As String = CreateTempDir("file[123].mkv")
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("file1.mkv", False))
                Assert.IsTrue(filter.IsIgnored("file2.mkv", False))
                Assert.IsTrue(filter.IsIgnored("file3.mkv", False))
                Assert.IsFalse(filter.IsIgnored("file4.mkv", False))
                Assert.IsFalse(filter.IsIgnored("file.mkv", False))
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

#End Region

#Region "Real-world Plex scenarios"

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_RealWorld_ExcludeSampleAndTrailer()
            Dim content As String = String.Join(vbLf, New String() {
                "# Exclude sample and trailer files",
                "*sample*",
                "*trailer*",
                "behind the scenes/"
            })
            Dim dir As String = CreateTempDir(content)
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("movie-sample.mkv", False))
                Assert.IsTrue(filter.IsIgnored("sample.mp4", False))
                Assert.IsTrue(filter.IsIgnored("movie-trailer.mkv", False))
                Assert.IsTrue(filter.IsIgnored("behind the scenes", True))
                Assert.IsFalse(filter.IsIgnored("behind the scenes", False), "Dir-only should not match file")
                Assert.IsFalse(filter.IsIgnored("movie.mkv", False))
                Assert.IsFalse(filter.IsIgnored("Season 01", True))
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_RealWorld_ExcludeTorrentPartFiles()
            Dim content As String = String.Join(vbLf, New String() {
                "*.!ut",
                "*.part",
                "~*PartFile*"
            })
            Dim dir As String = CreateTempDir(content)
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("movie.mkv.!ut", False))
                Assert.IsTrue(filter.IsIgnored("episode.s01e01.mkv.part", False))
                Assert.IsTrue(filter.IsIgnored("~BitTorrentPartFile_588F7342.dat", False))
                Assert.IsFalse(filter.IsIgnored("movie.mkv", False))
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PlexIgnore_RealWorld_ExcludeExtrasFolder()
            Dim content As String = String.Join(vbLf, New String() {
                "extras/",
                "featurettes/",
                "behind the scenes/"
            })
            Dim dir As String = CreateTempDir(content)
            Try
                Dim filter As New PlexIgnoreFilter(dir)

                Assert.IsTrue(filter.IsIgnored("extras", True))
                Assert.IsTrue(filter.IsIgnored("featurettes", True))
                Assert.IsTrue(filter.IsIgnored("behind the scenes", True))
                Assert.IsFalse(filter.IsIgnored("Season 1", True))
                Assert.IsFalse(filter.IsIgnored("extras", False), "File named 'extras' should not be ignored")
            Finally
                CleanupTempDir(dir)
            End Try
        End Sub

#End Region

    End Class

End Namespace
