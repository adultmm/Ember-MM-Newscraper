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

Imports System.Drawing
Imports System.IO
Imports EmberAPI
Imports NLog

Public Class PosterEditor
    Implements Interfaces.GenericModule

#Region "Fields"

    Shared logger As Logger = NLog.LogManager.GetCurrentClassLogger()


    'FIXME: !!! make these configurable
    Friend Shared _saveUntouchedPosters As Boolean = True
    'FIXME: it's too agressive (delete poster, it regenerates immediately; maybe after poster scrape?)
    Friend Shared _autoGeneratePoster As Boolean = False
    Friend Shared _autoGeneratePosterTemplatePath As String = Nothing
    Friend Shared _autoGenerateWithDefaultTemplate As Boolean = False
    Friend Shared _showTestTemplate As Boolean = True


    Private WithEvents MyMenu As New System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MyTrayMenu As New System.Windows.Forms.ToolStripMenuItem
    Private _AssemblyName As String = String.Empty
    Private _enabled As Boolean = False
    Private _name As String = "Poster Editor"
    Private _setup As frmSettingsHolder
    '    Private frmTV As frmTVExtrator
    Private frmMoviePosterEditor As frmMoviePosterEditor

#End Region 'Fields

#Region "Events"

    Public Event GenericEvent(ByVal mType As Enums.ModuleEventType, ByRef _params As System.Collections.Generic.List(Of Object)) Implements Interfaces.GenericModule.GenericEvent

    Public Event ModuleEnabledChanged(ByVal Name As String, ByVal State As Boolean, ByVal diffOrder As Integer) Implements Interfaces.GenericModule.ModuleSetupChanged

    Public Event ModuleSettingsChanged() Implements Interfaces.GenericModule.ModuleSettingsChanged

    Public Event SetupNeedsRestart() Implements Interfaces.GenericModule.SetupNeedsRestart

#End Region 'Events

#Region "Properties"

    Public Property Enabled() As Boolean Implements Interfaces.GenericModule.Enabled
        Get
            Return _enabled
        End Get
        Set(ByVal value As Boolean)
            If _enabled = value Then Return
            _enabled = value
            If _enabled Then
                Enable()
            Else
                Disable()
            End If
        End Set
    End Property

    ReadOnly Property IsBusy() As Boolean Implements Interfaces.GenericModule.IsBusy
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property ModuleName() As String Implements Interfaces.GenericModule.ModuleName
        Get
            Return _name
        End Get
    End Property

    Public ReadOnly Property ModuleType() As List(Of Enums.ModuleEventType) Implements Interfaces.GenericModule.ModuleType
        Get
            Return New List(Of Enums.ModuleEventType)(New Enums.ModuleEventType() {Enums.ModuleEventType.EditorTabPopulate_Movie, Enums.ModuleEventType.AfterEdit_Movie,
                Enums.ModuleEventType.OnPosterChangedDuringEdit_Movie, Enums.ModuleEventType.OnBeforeSave_Movie})
        End Get
    End Property

    Public ReadOnly Property ModuleVersion() As String Implements Interfaces.GenericModule.ModuleVersion
        Get
            Return FileVersionInfo.GetVersionInfo(Reflection.Assembly.GetExecutingAssembly.Location).FileVersion.ToString
        End Get
    End Property

#End Region 'Properties

#Region "Methods"

    Public Sub Init(ByVal sAssemblyName As String, ByVal sExecutable As String) Implements Interfaces.GenericModule.Init
        _AssemblyName = sAssemblyName
        'Master.eLang.LoadLanguage(Master.eSettings.Language, sExecutable)
    End Sub

    Public Function InjectSetup() As Containers.SettingsPanel Implements Interfaces.GenericModule.InjectSetup
        Dim SPanel As New Containers.SettingsPanel
        _setup = New frmSettingsHolder
        _setup.cbEnabled.Checked = _enabled
        SPanel.Name = _name
        'FIXME: new string
        SPanel.Text = Master.eLang.GetString(310, "Frame Extractor")
        'FIXME: where is it used?
        SPanel.Prefix = "Extrator_"
        SPanel.Type = Master.eLang.GetString(802, "Modules")
        SPanel.ImageIndex = If(_enabled, 9, 10)
        SPanel.Order = 100
        SPanel.Panel = _setup.pnlSettings()
        AddHandler _setup.ModuleEnabledChanged, AddressOf Handle_SetupChanged
        AddHandler _setup.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged
        Return SPanel
    End Function

    Public Function RunGeneric(ByVal mType As Enums.ModuleEventType, ByRef _params As List(Of Object), ByRef _singleobjekt As Object, ByRef _dbelement As Database.DBElement) As Interfaces.ModuleResult Implements Interfaces.GenericModule.RunGeneric
        Select Case mType
            Case Enums.ModuleEventType.EditorTabPopulate_Movie
                'FIXME: test the disabled case
                If Master.eSettings.MoviePosterAnyEnabled Then
                    frmMoviePosterEditor = New frmMoviePosterEditor(Me)
                    frmMoviePosterEditor.Init(_dbelement)
                    _params.Add(frmMoviePosterEditor)
                End If
            Case Enums.ModuleEventType.AfterEdit_Movie, Enums.ModuleEventType.OnBeforeSave_Movie
                If mType = Enums.ModuleEventType.OnBeforeSave_Movie AndAlso Not DirectCast(_params(1), Boolean) Then
                    Return New Interfaces.ModuleResult
                End If
                If _dbelement.ImagesContainer.Poster.LoadAndCache(Enums.ContentType.Movie, True) Then
                    If ExifHelper.HasExifInfo(_dbelement.ImagesContainer.Poster.ImageOriginal.Image) Then
                        If _saveUntouchedPosters Then
                            Dim untouchedTmpPath As String = PosterTemplateHelper.GetUntouchedPosterTempPath(_dbelement)
                            If File.Exists(untouchedTmpPath) Then
                                File.Copy(untouchedTmpPath, PosterTemplateHelper.GetUntouchedPathForMovie(_dbelement, Path.GetExtension(untouchedTmpPath)))
                            Else
                                logger.Error("Untouched poster not found in temp folder: {0}, for movie: {1}", untouchedTmpPath, _dbelement.Filename)
                            End If
                        End If
                    Else
                        AutoGeneratePoster(_dbelement)
                    End If
                Else
                    AutoGeneratePoster(_dbelement)
                End If
            Case Enums.ModuleEventType.OnPosterChangedDuringEdit_Movie
                If TypeOf _singleobjekt IsNot frmMoviePosterEditor Then
                    If frmMoviePosterEditor IsNot Nothing Then
                        frmMoviePosterEditor.OnPosterChangedDuringEdit(_dbelement)
                    End If
                End If
        End Select
    End Function

    Private Shared Function AutoGeneratePoster(_dbelement As Database.DBElement) As Boolean
        If _autoGeneratePoster AndAlso Not ExifHelper.HasExifInfo(_dbelement.ImagesContainer.Poster.ImageOriginal.Image) Then
            Dim foundTemplate = PosterTemplateHelper.DetectTemplate(_dbelement, True)
            If foundTemplate IsNot Nothing Then
                Dim newPoster As Image = Nothing
                Try
                    newPoster = PosterTemplateHelper.ApplyTemplateToPoster(_dbelement, foundTemplate, True, False)
                    _dbelement.ImagesContainer.Poster.ImageOriginal.UpdateMSfromImg(newPoster)
                    Return True
                Finally
                    PosterTemplateHelper.SafeDispose(newPoster)
                End Try
            End If
        End If
        Return False
    End Function

    'Forms (of our module) can use this method to raise GenericEvents
    Sub RaiseGenericEvent(ByVal mType As Enums.ModuleEventType, ByRef _params As List(Of Object))
        RaiseEvent GenericEvent(mType, _params)
    End Sub

    Public Sub SaveSetup(ByVal DoDispose As Boolean) Implements Interfaces.GenericModule.SaveSetup
        'Master.eSettings.XBMCComs.AddRange(_MySettings.XComs)
        Enabled = _setup.cbEnabled.Checked
        If DoDispose Then
            RemoveHandler _setup.ModuleEnabledChanged, AddressOf Handle_SetupChanged
            RemoveHandler _setup.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged
            _setup.Dispose()
        End If
    End Sub

    Sub Disable()
        Try

        Catch ex As Exception
        End Try
    End Sub

    Sub Enable()
        Try
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Handle_ModuleSettingsChanged()
        RaiseEvent ModuleSettingsChanged()
    End Sub

    Private Sub Handle_SetupChanged(ByVal state As Boolean, ByVal difforder As Integer)
        RaiseEvent ModuleEnabledChanged(_name, state, difforder)
    End Sub

#End Region 'Methods

End Class