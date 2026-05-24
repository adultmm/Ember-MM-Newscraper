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
Imports System.Text
Imports System.Text.RegularExpressions
Imports NLog

''' <summary>
''' Parses and applies .plexignore files, using the same rules as Plex Media Server.
''' A .plexignore file in a directory applies to the contents of that directory only.
''' Syntax follows .gitignore conventions: glob patterns, # comments, ! negation.
''' </summary>
Public Class PlexIgnoreFilter

#Region "Fields"

        Shared logger As Logger = LogManager.GetCurrentClassLogger()

        Private ReadOnly _patterns As New List(Of PlexIgnorePattern)
        Private ReadOnly _hasPatterns As Boolean

#End Region 'Fields

#Region "Constructors"

        ''' <summary>
        ''' Loads the .plexignore file from the given directory, if it exists.
        ''' Pass an empty string to create a no-op filter (nothing will be ignored).
        ''' </summary>
        Public Sub New(ByVal directoryPath As String)
            If String.IsNullOrEmpty(directoryPath) Then Return
            Dim ignoreFile As String = Path.Combine(directoryPath, ".plexignore")
            If File.Exists(ignoreFile) Then
                Try
                    For Each line As String In File.ReadAllLines(ignoreFile, Encoding.UTF8)
                        Dim trimmed As String = line.Trim()
                        ' Skip empty lines and comments
                        If String.IsNullOrEmpty(trimmed) OrElse trimmed.StartsWith("#") Then Continue For

                        Dim isNegation As Boolean = trimmed.StartsWith("!")
                        Dim pattern As String = If(isNegation, trimmed.Substring(1).Trim(), trimmed)

                        ' Skip blank pattern after stripping !
                        If String.IsNullOrEmpty(pattern) Then Continue For

                        ' Trailing spaces are ignored unless escaped with backslash
                        pattern = StripTrailingSpaces(pattern)

                        ' Determine if this pattern matches only directories (trailing /)
                        Dim dirOnly As Boolean = pattern.EndsWith("/")
                        If dirOnly Then pattern = pattern.TrimEnd("/"c)

                        ' Determine if pattern is anchored (contains / other than trailing)
                        Dim isAnchored As Boolean = pattern.Contains("/")

                        _patterns.Add(New PlexIgnorePattern With {
                            .IsNegation = isNegation,
                            .DirectoryOnly = dirOnly,
                            .IsAnchored = isAnchored,
                            .Regex = BuildRegex(pattern, isAnchored)
                        })
                    Next
                    _hasPatterns = _patterns.Count > 0
                Catch ex As Exception
                    logger.Warn(String.Format("[PlexIgnoreFilter] Could not read .plexignore in ""{0}"": {1}", directoryPath, ex.Message))
                End Try
            End If
        End Sub

#End Region 'Constructors

#Region "Methods"

        ''' <summary>
        ''' True when a .plexignore file was found and contained at least one rule.
        ''' </summary>
        Public ReadOnly Property HasActiveRules As Boolean
            Get
                Return _hasPatterns
            End Get
        End Property

        ''' <summary>
        ''' Returns True if the given file or directory name should be ignored
        ''' according to the .plexignore rules loaded for this directory.
        ''' </summary>
        ''' <param name="name">The file or directory name (not full path).</param>
        ''' <param name="isDirectory">True if the entry is a directory.</param>
        Public Function IsIgnored(ByVal name As String, ByVal isDirectory As Boolean) As Boolean
            If Not _hasPatterns Then Return False

            Dim result As Boolean = False
            For Each p As PlexIgnorePattern In _patterns
                ' Directory-only patterns do not apply to files
                If p.DirectoryOnly AndAlso Not isDirectory Then Continue For

                If p.Regex.IsMatch(name) Then
                    result = Not p.IsNegation
                End If
            Next
            Return result
        End Function

        ''' <summary>
        ''' Returns True if the file at the given full path is excluded by a .plexignore
        ''' file in the same directory as the file.
        ''' </summary>
        Public Shared Function IsIgnoredFilePath(ByVal filePath As String) As Boolean
            If String.IsNullOrEmpty(filePath) Then Return False
            Dim fileDir As String = Path.GetDirectoryName(filePath)
            If String.IsNullOrEmpty(fileDir) Then Return False
            Dim filter As New PlexIgnoreFilter(fileDir)
            Return filter.IsIgnored(Path.GetFileName(filePath), False)
        End Function

        ''' <summary>
        ''' Strips trailing unescaped spaces from a pattern line.
        ''' </summary>
        Private Shared Function StripTrailingSpaces(ByVal s As String) As String
            Dim i As Integer = s.Length - 1
            While i >= 0 AndAlso s(i) = " "c
                If i > 0 AndAlso s(i - 1) = "\"c Then
                    Exit While
                End If
                i -= 1
            End While
            Return s.Substring(0, i + 1)
        End Function

        ''' <summary>
        ''' Converts a glob pattern to a Regex.
        ''' Rules:
        '''   **  matches any number of path segments (including zero)
        '''   *   matches any characters except /
        '''   ?   matches any single character except /
        '''   [abc] character class — passed through as-is
        '''   Everything else is escaped.
        ''' For non-anchored patterns (no / in pattern) the regex matches only the
        ''' filename part, so it is implicitly anchored at start and end.
        ''' For anchored patterns the full relative path is matched.
        ''' </summary>
        Private Shared Function BuildRegex(ByVal pattern As String, ByVal isAnchored As Boolean) As Regex
            Dim sb As New StringBuilder("^")
            Dim i As Integer = 0
            While i < pattern.Length
                Dim c As Char = pattern(i)
                If c = "*"c AndAlso i + 1 < pattern.Length AndAlso pattern(i + 1) = "*"c Then
                    ' ** — match anything including slashes
                    sb.Append(".*")
                    i += 2
                    ' skip optional surrounding slashes
                    If i < pattern.Length AndAlso pattern(i) = "/"c Then i += 1
                ElseIf c = "*"c Then
                    sb.Append("[^/\\]*")
                    i += 1
                ElseIf c = "?"c Then
                    sb.Append("[^/\\]")
                    i += 1
                ElseIf c = "["c Then
                    ' Find closing ] and pass the character class through
                    Dim j As Integer = pattern.IndexOf("]"c, i + 1)
                    If j >= 0 Then
                        sb.Append(pattern.Substring(i, j - i + 1))
                        i = j + 1
                    Else
                        sb.Append(Regex.Escape(c.ToString()))
                        i += 1
                    End If
                ElseIf c = "/"c Then
                    sb.Append("[/\\]")
                    i += 1
                Else
                    sb.Append(Regex.Escape(c.ToString()))
                    i += 1
                End If
            End While
            sb.Append("$")
            Return New Regex(sb.ToString(), RegexOptions.IgnoreCase Or RegexOptions.Compiled)
        End Function

#End Region 'Methods

#Region "Nested Types"

        Private Class PlexIgnorePattern
            Public Property IsNegation As Boolean
            Public Property DirectoryOnly As Boolean
            Public Property IsAnchored As Boolean
            Public Property Regex As Regex
        End Class

#End Region 'Nested Types

End Class
