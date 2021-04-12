Imports System.IO

Public Class Properties

    Private m_Properties As New Hashtable

    Public Sub New()

    End Sub

    Private Sub Add(ByVal key As String, ByVal value As String)
        m_Properties.Add(key, value)
    End Sub

    Public Sub Load(ByRef sr As Stream)

        Dim line As String
        Dim key As String
        Dim value As String
        Dim reader = New StreamReader(sr)
        Do While reader.Peek <> -1
            line = reader.ReadLine
            If line = Nothing OrElse line.Length = 0 OrElse line.StartsWith("#") Then
                Continue Do
            End If

            Dim v As String() = line.Split("="c)
            key = v(0)
            value = v(1)

            Add(key, value)
        Loop
    End Sub

    Public Function GetProperty(ByVal key As String) As String
        Dim value = m_Properties.Item(key)
        Return If(value IsNot Nothing, value.ToString, Nothing)
    End Function

    Public Function GetProperty(ByVal key As String, ByVal defValue As String) As String

        Dim value As String = GetProperty(key)
        If value = Nothing Then
            value = defValue
        End If

        Return value

    End Function

End Class