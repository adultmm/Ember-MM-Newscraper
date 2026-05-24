' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports System.Windows.Forms
Imports EmberAPI
Imports Microsoft.VisualBasic.Constants

Public Module PlexIgnorePromptHelper

    ''' <summary>
    ''' Returns True if the caller should schedule a deferred PlexIgnore clean after Settings closes.
    ''' </summary>
    Public Function PromptAfterMovieSourceSave(ByVal loadedUsePlexIgnore As Boolean, ByVal savedUsePlexIgnore As Boolean, ByVal sourceId As Long) As Boolean
        If Not ShouldShowPrompt(loadedUsePlexIgnore, savedUsePlexIgnore, Master.eSettings.MovieAskPlexIgnoreCleanPrompt) Then Return False
        If Not Master.DB.HasIndexedMovies() Then Return False
        If Not Master.DB.HasIndexedMovies(sourceId) Then Return False
        Return ShowPrompt(True)
    End Function

    ''' <summary>
    ''' Returns True if the caller should schedule a deferred PlexIgnore clean after Settings closes.
    ''' </summary>
    Public Function PromptAfterTVSourceSave(ByVal loadedUsePlexIgnore As Boolean, ByVal savedUsePlexIgnore As Boolean, ByVal sourceId As Long) As Boolean
        If Not ShouldShowPrompt(loadedUsePlexIgnore, savedUsePlexIgnore, Master.eSettings.TVAskPlexIgnoreCleanPrompt) Then Return False
        If Not Master.DB.HasIndexedTVEpisodes() Then Return False
        If Not Master.DB.HasIndexedTVEpisodes(sourceId) Then Return False
        Return ShowPrompt(False)
    End Function

    Private Function ShouldShowPrompt(ByVal loadedUsePlexIgnore As Boolean, ByVal savedUsePlexIgnore As Boolean, ByVal askPrompt As Boolean) As Boolean
        If loadedUsePlexIgnore OrElse Not savedUsePlexIgnore Then Return False
        Return askPrompt
    End Function

    Private Function ShowPrompt(ByVal isMovie As Boolean) As Boolean
        'FIXME: i18n
        Dim message As String = String.Concat(
            "You have enabled .plexignore for this source.", vbCrLf, vbCrLf,
            "Library scans will skip files excluded by .plexignore rules, but entries that were already indexed will remain in the database until they are removed.", vbCrLf, vbCrLf,
            "Would you like to remove them now?", vbCrLf, vbCrLf,
            "Only database entries are removed; files on disk are not deleted.", vbCrLf, vbCrLf,
            "You can also remove them later with Tools -> Clean Database, then run a new library scan.")

        'FIXME: i18n
        Select Case MessageBox.Show(message, ".plexignore enabled", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
            Case DialogResult.Yes
                Return True
            Case DialogResult.Cancel
                If isMovie Then
                    Master.eSettings.MovieAskPlexIgnoreCleanPrompt = False
                Else
                    Master.eSettings.TVAskPlexIgnoreCleanPrompt = False
                End If
                Master.eSettings.Save()
                Return False
            Case Else
                Return False
        End Select
    End Function

End Module
