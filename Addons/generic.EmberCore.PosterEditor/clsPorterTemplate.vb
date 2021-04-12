Imports NLog
Imports System.Drawing
Imports System.Xml.Serialization
Imports generic.EmberCore.PosterEditor
Imports System.IO
Imports System.Text

<Serializable()>
Public Class PosterTemplate

    Public Shared ReadOnly DEFAULT_BACKGROUND_COLOR As Color = Color.Black

    Shared logger As Logger = NLog.LogManager.GetCurrentClassLogger()

    Private _backgroundColor As Color = DEFAULT_BACKGROUND_COLOR

    Public Property CropEnabled() As Boolean
    Public Property Image As ImageSettings = New ImageSettings()
    Public Property PosterRatio As AspectRatio = New AspectRatio(1.5, "Default")


    <XmlIgnore>
    Public Property BackgroundColor As Color
        Get
            Return _backgroundColor
        End Get
        Set(value As Color)
            _backgroundColor = value
        End Set
    End Property

    <XmlElement("BackgroundColor")>
    Public Property BackgroundColorHTML As String
        Get
            Return If(_backgroundColor <> Nothing, ColorTranslator.ToHtml(_backgroundColor), Nothing)
        End Get
        Set(value As String)
            Try
                _backgroundColor = If(String.IsNullOrWhiteSpace(value), DEFAULT_BACKGROUND_COLOR, ColorTranslator.FromHtml(value))
            Catch ex As Exception
                logger.Error(ex, "Illegal color value: {0}", value)
                _backgroundColor = DEFAULT_BACKGROUND_COLOR
            End Try
        End Set
    End Property

    Public Shared Function Load(path As String) As PosterTemplate
        Using reader = New StreamReader(New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            Dim serializer As New XmlSerializer(GetType(PosterTemplate))
            Return CType(serializer.Deserialize(reader), PosterTemplate)
        End Using
    End Function

    Public Shared Sub Save(template As PosterTemplate, templateSavePath As String)
        Using writer As StreamWriter = New StreamWriter(templateSavePath)
            Dim xmlSer = New Xml.Serialization.XmlSerializer(GetType(PosterTemplate))
            xmlSer.Serialize(writer, template)
        End Using
    End Sub

    <Serializable()>
    Public Class ImageSettings
        Public Property Filename() As String
        Public Property HorizontalAlignment() As HorizontalAlignment
        Public Property VerticalAlignment() As VerticalAlignment
        Public Property WidthPercent As Double = 50.0
        Public Property Enabled As Boolean
    End Class

    <Serializable()>
    Public Class AspectRatio
        Implements IEquatable(Of AspectRatio)

        Public Sub New()

        End Sub

        Public Sub New(ratio As Double, ratioLabel As String)
            Me.Ratio = ratio
            Me.RatioLabel = ratioLabel
        End Sub

        Public Property Ratio As Double
        Public Property RatioLabel As String

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is AspectRatio Then
                Return Equals(CType(obj, AspectRatio))
            End If
            Return False
        End Function

        Public Overloads Function Equals(other As AspectRatio) As Boolean Implements IEquatable(Of AspectRatio).Equals
            If ReferenceEquals(Me, other) Then
                Return True
            End If
            If ReferenceEquals(Nothing, other) Then
                Return False
            End If

            If (Ratio <> other.Ratio) Then
                Return False
            End If

            If RatioLabel <> other.RatioLabel Then
                Return False
            End If

            Return True
        End Function
    End Class

    Public Enum HorizontalAlignment
        Left = 0
        Right = 1
        Center = 2
        Stretch = 3
    End Enum

    Public Enum VerticalAlignment
        Top = 0
        Bottom = 1
        Middle = 2
        Stretch = 3
    End Enum

End Class



Public Class PosterTemplateWithPath

    Public ReadOnly Property Template As PosterTemplate
    Public ReadOnly Property Path As String

    Sub New(template As PosterTemplate, path As String)
        If template Is Nothing Then
            Throw New ArgumentNullException("template")
        End If
        If path Is Nothing Then
            Throw New ArgumentNullException("path")
        End If
        Me._Template = template
        Me._Path = path
    End Sub

End Class
