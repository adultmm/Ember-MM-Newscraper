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

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports EmberAPI

Namespace EmberTests

    <TestClass()>
    Public Class Test_PartialDownloadFilter

        Private Const DefaultPattern As String = PartialDownloadFilter.DefaultPattern

#Region "Methods"

        <UnitTest>
        <TestMethod()>
        Public Sub PartialDownload_BitTorrentPartFileDat_Matches()
            Assert.IsTrue(PartialDownloadFilter.IsPartialDownload("~BitTorrentPartFile_1EDE6584BF.dat", DefaultPattern))
            Assert.IsTrue(PartialDownloadFilter.IsPartialDownload("~uTorrentPartFile_588F7342.dat", DefaultPattern))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PartialDownload_UTorrentSuffixes_Match()
            Assert.IsTrue(PartialDownloadFilter.IsPartialDownload("movie.mkv.part", DefaultPattern))
            Assert.IsTrue(PartialDownloadFilter.IsPartialDownload("episode.mkv.!ut", DefaultPattern))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PartialDownload_QBittorrentSuffix_Matches()
            Assert.IsTrue(PartialDownloadFilter.IsPartialDownload("movie.mkv.!qB", DefaultPattern))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PartialDownload_OtherClientSuffixes_Match()
            Assert.IsTrue(PartialDownloadFilter.IsPartialDownload("movie.mkv.bc!", DefaultPattern))
            Assert.IsTrue(PartialDownloadFilter.IsPartialDownload("movie.mkv.az!", DefaultPattern))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PartialDownload_NormalMedia_DoesNotMatch()
            Assert.IsFalse(PartialDownloadFilter.IsPartialDownload("movie.mkv", DefaultPattern))
            Assert.IsFalse(PartialDownloadFilter.IsPartialDownload("episode.s01e01.mp4", DefaultPattern))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PartialDownload_NormalDat_DoesNotMatch()
            Assert.IsFalse(PartialDownloadFilter.IsPartialDownload("disc.dat", DefaultPattern))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PartialDownload_CaseInsensitive()
            Assert.IsTrue(PartialDownloadFilter.IsPartialDownload("~bittorrentpartfile_abc.dat", DefaultPattern))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PartialDownload_CustomPattern_MatchesOnlyConfiguredSuffix()
            Dim customPattern As String = "\.!qB$"
            Assert.IsTrue(PartialDownloadFilter.IsPartialDownload("movie.mkv.!qB", customPattern))
            Assert.IsFalse(PartialDownloadFilter.IsPartialDownload("~BitTorrentPartFile_abc.dat", customPattern))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PartialDownload_InvalidPattern_FallsBackToDefault()
            Assert.IsTrue(PartialDownloadFilter.IsPartialDownload("~BitTorrentPartFile_abc.dat", "(invalid"))
            Assert.IsFalse(PartialDownloadFilter.IsPartialDownload("movie.mkv", "(invalid"))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub PartialDownload_EmptyFileName_DoesNotMatch()
            Assert.IsFalse(PartialDownloadFilter.IsPartialDownload(String.Empty, DefaultPattern))
            Assert.IsFalse(PartialDownloadFilter.IsPartialDownload(Nothing, DefaultPattern))
        End Sub

#End Region 'Methods

    End Class

End Namespace
