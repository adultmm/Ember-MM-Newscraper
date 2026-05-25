' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports System.IO
Imports NLog

''' <summary>
''' Sidecar file discovery and deletion for partial-download database clean.
''' </summary>
Public Class PartialDownloadSidecarHelper

#Region "Fields"

    Shared logger As Logger = LogManager.GetCurrentClassLogger()

#End Region 'Fields

#Region "Methods"

    Public Shared Function FilterSidecarFilePaths(ByVal mainFilePath As String, ByVal allItems As IEnumerable(Of FileSystemInfo)) As List(Of String)
        Dim sidecars As New List(Of String)
        If allItems Is Nothing Then Return sidecars
        If String.IsNullOrEmpty(mainFilePath) Then Return sidecars

        For Each item As FileSystemInfo In allItems
            If TypeOf item IsNot FileInfo Then Continue For
            If Not IsPartialSidecarFile(mainFilePath, item.FullName) Then Continue For
            If item.Exists Then sidecars.Add(item.FullName)
        Next

        sidecars.Sort(StringComparer.OrdinalIgnoreCase)
        Return sidecars
    End Function

    Public Shared Function FilterSidecarFilePaths(ByVal mainFilePath As String, ByVal candidatePaths As IEnumerable(Of String)) As List(Of String)
        Dim sidecars As New List(Of String)
        If candidatePaths Is Nothing Then Return sidecars
        If String.IsNullOrEmpty(mainFilePath) Then Return sidecars

        For Each candidatePath As String In candidatePaths
            If String.IsNullOrEmpty(candidatePath) Then Continue For
            If Not IsPartialSidecarFile(mainFilePath, candidatePath) Then Continue For
            If File.Exists(candidatePath) Then sidecars.Add(candidatePath)
        Next

        sidecars.Sort(StringComparer.OrdinalIgnoreCase)
        Return sidecars
    End Function

    Public Shared Function IsPartialSidecarFile(ByVal mainFilePath As String, ByVal candidatePath As String) As Boolean
        If String.IsNullOrEmpty(mainFilePath) OrElse String.IsNullOrEmpty(candidatePath) Then Return False
        If String.Equals(mainFilePath, candidatePath, StringComparison.OrdinalIgnoreCase) Then Return False

        Dim baseName As String = Path.GetFileNameWithoutExtension(mainFilePath)
        If String.IsNullOrEmpty(baseName) Then Return False

        Return Path.GetFileName(candidatePath).StartsWith(baseName, StringComparison.OrdinalIgnoreCase)
    End Function

    Public Shared Function GetExistingSidecarFiles(ByVal dbElement As Database.DBElement) As List(Of String)
        If dbElement Is Nothing OrElse Not dbElement.FilenameSpecified Then Return New List(Of String)

        Try
            Dim sidecars As List(Of String) = GetSidecarPathsFromFilenameList(dbElement)

            logger.Info(String.Format("[PartialDownloadSidecar] Found {0} sidecar file(s) for ""{1}""", sidecars.Count, dbElement.Filename))
            For Each sidecarPath As String In sidecars
                logger.Debug(String.Format("[PartialDownloadSidecar] Sidecar: ""{0}""", sidecarPath))
            Next
            Return sidecars
        Catch ex As Exception
            logger.Info(ex, String.Format("[PartialDownloadSidecar] Sidecar discovery failed for ""{0}""", dbElement.Filename))
            Return New List(Of String)
        End Try
    End Function

    Private Shared Function GetSidecarPathsFromFilenameList(ByVal dbElement As Database.DBElement) As List(Of String)
        Dim lstFiles As New List(Of String)
        Const forced As Boolean = True

        Select Case dbElement.ContentType
            Case Enums.ContentType.Movie
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainBanner, forced))
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainClearArt, forced))
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainClearLogo, forced))
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainDiscArt, forced))
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainFanart, forced))
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainLandscape, forced))
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainNFO, forced))
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainPoster, forced))
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainSubtitle, forced))
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainTheme, forced))
                lstFiles.AddRange(FileUtils.GetFilenameList.Movie(dbElement, Enums.ModifierType.MainTrailer, forced))

            Case Enums.ContentType.TVEpisode
                lstFiles.AddRange(FileUtils.GetFilenameList.TVEpisode(dbElement, Enums.ModifierType.EpisodeFanart))
                lstFiles.AddRange(FileUtils.GetFilenameList.TVEpisode(dbElement, Enums.ModifierType.EpisodeNFO))
                lstFiles.AddRange(FileUtils.GetFilenameList.TVEpisode(dbElement, Enums.ModifierType.EpisodePoster))
                lstFiles.AddRange(FileUtils.GetFilenameList.TVEpisode(dbElement, Enums.ModifierType.EpisodeSubtitle))
                lstFiles.AddRange(FileUtils.GetFilenameList.TVEpisode(dbElement, Enums.ModifierType.EpisodeWatchedFile))
        End Select

        Return FilterSidecarFilePaths(dbElement.Filename, lstFiles.Distinct(StringComparer.OrdinalIgnoreCase))
    End Function

    Public Shared Function TryDeleteFiles(ByVal filePaths As IEnumerable(Of String)) As Integer
        Dim deleted As Integer = 0
        If filePaths Is Nothing Then Return deleted

        For Each filePath As String In filePaths
            If String.IsNullOrEmpty(filePath) Then Continue For
            Try
                If File.Exists(filePath) Then
                    File.Delete(filePath)
                    deleted += 1
                    logger.Info(String.Format("[PartialDownloadSidecar] Deleted sidecar file ""{0}""", filePath))
                End If
            Catch ex As Exception
                logger.Info(ex, String.Format("[PartialDownloadSidecar] Failed to delete sidecar file ""{0}""", filePath))
            End Try
        Next

        Return deleted
    End Function

#End Region 'Methods

End Class
