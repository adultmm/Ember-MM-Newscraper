' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports System.Windows.Forms
Imports EmberAPI

Public Module PartialDownloadUpgradePromptHelper

    ''' <summary>
    ''' Handles the first-run upgrade prompt when ExcludePartialDownloadFiles was missing from Settings.xml.
    ''' Returns True if the caller should run a database clean (movies and TV).
    ''' </summary>
    Public Function TryHandleUpgradePrompt() As Boolean
        If Not Master.eSettings.PendingPartialDownloadUpgradePrompt Then Return False

        Master.eSettings.PendingPartialDownloadUpgradePrompt = False

        If Not Master.DB.HasIndexedMovies() AndAlso Not Master.DB.HasIndexedTVEpisodes() Then
            Master.eSettings.ExcludePartialDownloadFiles = True
            Master.eSettings.PartialDownloadExcludePattern = PartialDownloadFilter.DefaultPattern
            PartialDownloadFilter.InvalidateCache()
            Master.eSettings.Save()
            Return False
        End If

        Dim message As String = String.Join(Environment.NewLine, New String() {
            "Partial download files (BitTorrent, uTorrent, qBittorrent, etc.) can now be excluded from library scans.", 'FIXME: i18n
            String.Empty,
            "Matching files will be skipped during scans. Entries already in the database can be removed with a database clean.", 'FIXME: i18n
            String.Empty,
            "Would you like to enable this feature now?", 'FIXME: i18n
            String.Empty,
            "You can change this later in Settings -> File System -> Partial Download Files." 'FIXME: i18n
        })

        If MessageBox.Show(message, "Partial Download Files", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then 'FIXME: i18n
            Master.eSettings.ExcludePartialDownloadFiles = True
            Master.eSettings.PartialDownloadExcludePattern = PartialDownloadFilter.DefaultPattern
            PartialDownloadFilter.InvalidateCache()
            Master.eSettings.Save()
            Return True
        End If

        Master.eSettings.ExcludePartialDownloadFiles = False
        If String.IsNullOrEmpty(Master.eSettings.PartialDownloadExcludePattern) Then
            Master.eSettings.PartialDownloadExcludePattern = PartialDownloadFilter.DefaultPattern
        End If
        PartialDownloadFilter.InvalidateCache()
        Master.eSettings.Save()
        Return False
    End Function

End Module
