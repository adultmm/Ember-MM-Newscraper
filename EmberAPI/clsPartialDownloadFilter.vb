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

Imports System.Text.RegularExpressions
Imports NLog

''' <summary>
''' Detects incomplete download / torrent client artifact file names during scan and database clean.
''' </summary>
Public Class PartialDownloadFilter

#Region "Fields"

        Shared logger As Logger = LogManager.GetCurrentClassLogger()

        Private Shared _cachedPattern As String = Nothing
        Private Shared _cachedRegex As Regex = Nothing

#End Region 'Fields

#Region "Constants"

        Public Const DefaultPattern As String = "^(~.*PartFile.*|.*\.(?:part|!ut|!qB|!bt|bc!|bt!|az!))$"

#End Region 'Constants

#Region "Methods"

        Public Shared Sub InvalidateCache()
            _cachedPattern = Nothing
            _cachedRegex = Nothing
        End Sub

        Public Shared Function IsPartialDownload(ByVal fileName As String) As Boolean
            If String.IsNullOrEmpty(fileName) Then Return False
            Return GetRegex().IsMatch(fileName)
        End Function

        ''' <summary>
        ''' Test-friendly overload with an explicit pattern (does not read Master.eSettings).
        ''' </summary>
        Public Shared Function IsPartialDownload(ByVal fileName As String, ByVal pattern As String) As Boolean
            If String.IsNullOrEmpty(fileName) Then Return False
            Return CreateRegex(pattern).IsMatch(fileName)
        End Function

        Private Shared Function GetRegex() As Regex
            Dim pattern As String = Master.eSettings.PartialDownloadExcludePattern
            If String.IsNullOrWhiteSpace(pattern) Then
                pattern = DefaultPattern
            Else
                pattern = pattern.Trim()
            End If

            If _cachedRegex IsNot Nothing AndAlso String.Equals(_cachedPattern, pattern, StringComparison.Ordinal) Then
                Return _cachedRegex
            End If

            _cachedRegex = CreateRegex(pattern)
            _cachedPattern = pattern
            Return _cachedRegex
        End Function

        Private Shared Function CreateRegex(ByVal pattern As String) As Regex
            Dim usePattern As String = pattern
            If String.IsNullOrWhiteSpace(usePattern) Then
                usePattern = DefaultPattern
            Else
                usePattern = usePattern.Trim()
            End If

            Try
                Return New Regex(usePattern, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
            Catch ex As Exception
                logger.Warn(String.Format("[PartialDownloadFilter] Invalid pattern ""{0}"": {1}. Using default.", usePattern, ex.Message))
                Return New Regex(DefaultPattern, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
            End Try
        End Function

#End Region 'Methods

End Class
