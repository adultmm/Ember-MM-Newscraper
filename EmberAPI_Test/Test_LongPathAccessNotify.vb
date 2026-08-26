' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports System.IO
Imports EmberAPI
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace EmberTests

    <TestClass()>
    Public Class Test_LongPathAccessNotify

        Private Const HResultPathTooLong As Integer = &H800700CE

        <UnitTest>
        <TestMethod()>
        Public Sub Strict_Matches_PathTooLongException()
            Dim ex As New PathTooLongException("The specified path, file name, or both are too long.")
            Assert.IsTrue(LongPathAccessNotify.IsStrictLongPathRelatedException(ex))
            Assert.AreEqual("PathTooLongException", LongPathAccessNotify.GetDetectionReason(ex))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub Strict_Matches_HRESULT_PATH_TOO_LONG()
            Dim ex As Exception = New HResultTestException(HResultPathTooLong)
            Assert.IsTrue(LongPathAccessNotify.IsStrictLongPathRelatedException(ex))
            Assert.AreEqual("HRESULT_PATH_TOO_LONG", LongPathAccessNotify.GetDetectionReason(ex))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub Strict_Ignores_Message_TooLong_FalsePositive()
            ' HTTP / validation messages must not trip FirstChance strict filter
            Dim ex As New Exception("Request URI too long")
            Assert.IsFalse(LongPathAccessNotify.IsStrictLongPathRelatedException(ex))
            Assert.IsTrue(String.IsNullOrEmpty(LongPathAccessNotify.GetDetectionReason(ex)))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub Broad_Ignores_Bare_TooLong_Message_Without_Path_Evidence()
            Dim ex As New InvalidOperationException("Value is too long for this field")
            Assert.IsFalse(LongPathAccessNotify.IsLongPathRelatedException(ex))
            Assert.IsTrue(String.IsNullOrEmpty(LongPathAccessNotify.GetDetectionReason(ex)))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub Broad_Matches_IOException_With_Long_Context_Path_When_OS_LongPaths_Off()
            If LongPathAccessNotify.IsWin32LongPathsEnabled() Then
                Assert.Inconclusive("OS long paths already enabled; io-with-long-context-path heuristic is gated off.")
            End If

            Dim longPath As String = "C:\" & New String("a"c, 260)
            Dim ex As New DirectoryNotFoundException("Could not find a part of the path.")
            Assert.AreEqual("io-with-long-context-path", LongPathAccessNotify.GetDetectionReason(ex, longPath))
            Assert.IsTrue(LongPathAccessNotify.IsLongPathRelatedException(ex, longPath))
            ' FirstChance must stay strict: context path is not available there
            Assert.IsFalse(LongPathAccessNotify.IsStrictLongPathRelatedException(ex))
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub Strict_Walks_InnerException()
            Dim inner As New PathTooLongException("inner")
            Dim outer As New InvalidOperationException("wrapper", inner)
            Assert.IsTrue(LongPathAccessNotify.IsStrictLongPathRelatedException(outer))
            Assert.AreEqual("PathTooLongException", LongPathAccessNotify.GetDetectionReason(outer))
        End Sub

        ''' <summary>Derived type so protected HResult setter is accessible under .NET Framework.</summary>
        Private Class HResultTestException
            Inherits Exception

            Public Sub New(ByVal hr As Integer)
                MyBase.New("native path failure")
                HResult = hr
            End Sub
        End Class

    End Class

End Namespace
