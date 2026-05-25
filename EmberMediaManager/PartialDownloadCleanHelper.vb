' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports EmberAPI
Imports NLog

Public Module PartialDownloadCleanHelper

#Region "Fields"

    Private logger As Logger = LogManager.GetCurrentClassLogger()

#End Region 'Fields

#Region "Methods"

    Public Function RunClean(ByVal clean As Structures.ScanOrClean) As Structures.PartialDownloadCleanResult
        Dim result As New Structures.PartialDownloadCleanResult
        If Not Master.eSettings.ExcludePartialDownloadFiles Then Return result

        logger.Info("[PartialDownloadClean] Cleaning started")

        Dim candidates As List(Of Structures.PartialDownloadCleanCandidate) = Master.DB.GetPartialDownloadCandidates(clean.Movies, clean.TV)
        logger.Info(String.Format("[PartialDownloadClean] Found {0} partial download candidate(s)", candidates.Count))
        Dim skipPromptsForThisClean As Boolean = False
        Dim rememberedDeleteFiles As Boolean? = Nothing
        Dim tvDeleted As Boolean = False

        For Each candidate As Structures.PartialDownloadCleanCandidate In candidates
            Dim dbElement As Database.DBElement = LoadDBElement(candidate)
            If dbElement Is Nothing Then
                Master.DB.DeletePartialDownloadEntry(candidate)
                IncrementDeletedCount(result, candidate.ContentType)
                If candidate.ContentType = Enums.ContentType.TVEpisode Then tvDeleted = True
                Continue For
            End If

            Dim sidecars As List(Of String) = PartialDownloadSidecarHelper.GetExistingSidecarFiles(dbElement)
            If sidecars.Count = 0 Then
                logger.Info(String.Format("[PartialDownloadClean] No sidecars for ""{0}"", removing DB entry only", candidate.FilePath))
                Master.DB.DeletePartialDownloadEntry(candidate)
                IncrementDeletedCount(result, candidate.ContentType)
                If candidate.ContentType = Enums.ContentType.TVEpisode Then tvDeleted = True
                Continue For
            End If

            Dim deleteSidecars As Boolean
            If skipPromptsForThisClean AndAlso rememberedDeleteFiles.HasValue Then
                deleteSidecars = rememberedDeleteFiles.Value
            Else
                Using dlg As New dlgPartialDownloadSidecarClean
                    Dim displayTitle As String = If(String.IsNullOrEmpty(candidate.DisplayTitle), candidate.FilePath, candidate.DisplayTitle)
                    Dim choice As dlgPartialDownloadSidecarClean.SidecarChoice = dlg.ShowDialog(displayTitle, candidate.FilePath, sidecars)

                    If choice = dlgPartialDownloadSidecarClean.SidecarChoice.Abort Then
                        result.WasAborted = True
                        logger.Info("[PartialDownloadClean] Cleaning aborted by user")
                        Exit For
                    End If

                    deleteSidecars = (choice = dlgPartialDownloadSidecarClean.SidecarChoice.DeleteFiles)
                    If dlg.ApplyToAllRemaining Then
                        skipPromptsForThisClean = True
                        rememberedDeleteFiles = deleteSidecars
                    End If
                End Using
            End If

            If deleteSidecars Then
                result.SidecarFilesDeleted += PartialDownloadSidecarHelper.TryDeleteFiles(sidecars)
            End If

            Master.DB.DeletePartialDownloadEntry(candidate)
            IncrementDeletedCount(result, candidate.ContentType)
            If candidate.ContentType = Enums.ContentType.TVEpisode Then tvDeleted = True
        Next

        If tvDeleted Then
            Master.DB.Delete_Empty_TVSeasons(-1, True)
        End If

        logger.Info(String.Format("[PartialDownloadClean] Cleaning done (movies={0}, episodes={1}, sidecars={2}, aborted={3})",
                                  result.MovieDeleted, result.TVDeleted, result.SidecarFilesDeleted, result.WasAborted))
        Return result
    End Function

    Private Function LoadDBElement(ByVal candidate As Structures.PartialDownloadCleanCandidate) As Database.DBElement
        Try
            Select Case candidate.ContentType
                Case Enums.ContentType.Movie
                    Return Master.DB.Load_Movie(candidate.Id)
                Case Enums.ContentType.TVEpisode
                    Return Master.DB.Load_TVEpisode(candidate.Id, True)
            End Select
        Catch ex As Exception
            logger.Debug(ex, String.Format("[PartialDownloadClean] Failed to load DB element id={0}", candidate.Id))
        End Try
        Return Nothing
    End Function

    Private Sub IncrementDeletedCount(ByRef result As Structures.PartialDownloadCleanResult, ByVal contentType As Enums.ContentType)
        Select Case contentType
            Case Enums.ContentType.Movie
                result.MovieDeleted += 1
            Case Enums.ContentType.TVEpisode
                result.TVDeleted += 1
        End Select
    End Sub

#End Region 'Methods

End Module
