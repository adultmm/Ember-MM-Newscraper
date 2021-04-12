Imports System.Drawing
Imports System.Runtime.Serialization


Public Class ExifHelper

    Private Const PROP_SoftwareUsed As Integer = &H131

    Private Const TYPE_AsciiString As Integer = 2

    Private Const POSTER_EDITOR_MARKER As String = "EmberMediaManager//PosterEditor"

    Public Shared Sub SetExifInfo(poster As Image)
        'Dim prop As Imaging.PropertyItem = poster.PropertyItems(0)
        Dim prop As Imaging.PropertyItem = CType(FormatterServices.GetUninitializedObject(GetType(Imaging.PropertyItem)), Imaging.PropertyItem)
        Dim data As Byte() = System.Text.Encoding.UTF8.GetBytes(POSTER_EDITOR_MARKER & vbNullChar)
        prop.Id = PROP_SoftwareUsed
        prop.Value = data
        prop.Type = TYPE_AsciiString
        prop.Len = data.Length
        poster.SetPropertyItem(prop)
    End Sub

    Public Shared Function HasExifInfo(poster As Image) As Boolean
        If poster Is Nothing Then
            Return False
        End If
        'GetPropertyItem throws an exception if we call with an unexisting id
        If CBool(Array.IndexOf(poster.PropertyIdList, PROP_SoftwareUsed) < 0) Then
            Return False
        End If
        Dim prop As Imaging.PropertyItem = poster.GetPropertyItem(PROP_SoftwareUsed)
        If prop Is Nothing Then
            Return False
        End If
        Dim result = System.Text.Encoding.UTF8.GetString(prop.Value)
        If result.EndsWith(vbNullChar) Then
            result = result.Substring(0, result.Length - 1)
        End If
        Return POSTER_EDITOR_MARKER.Equals(result)
    End Function

End Class
