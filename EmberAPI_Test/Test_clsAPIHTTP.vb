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

Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports EmberAPI


Namespace EmberTests

    <TestClass()>
    Public Class Test_clsAPIHTTP
        Implements IDisposable


        Dim HTTP As HTTP = Nothing
        ''' <summary>
        ''' Common setup for all tests in this class. This will run before EVERY test case in this class
        ''' </summary>
        ''' <remarks></remarks>
        <TestInitialize>
        Public Sub TestSetup()
            HTTP = New HTTP()
        End Sub

        ''' <summary>
        ''' Common teardown for all tests in this class. This will run after EVERY test case in this class
        ''' </summary>
        ''' <remarks></remarks>
        <TestCleanup>
        Public Sub TestCleanup()
            If HTTP IsNot Nothing Then
                HTTP.Dispose()
            End If
        End Sub
        <IntegrationTest>
        <TestMethod()>
        Public Sub HTTP_DownloadData()
            'Arrange

            'Act

            'Assert
            Assert.Inconclusive("Test not implemented")
        End Sub
        <IntegrationTest>
        <TestMethod()>
        Public Sub HTTP_PostDownloadData()
            'Arrange

            'Act

            'Assert
            Assert.Inconclusive("Test not implemented")
        End Sub
        <IntegrationTest>
        <TestMethod()>
        Public Sub HTTP_DownloadFile()
            'Arrange

            'Act

            'Assert
            Assert.Inconclusive("Test not implemented")
        End Sub
        <IntegrationTest>
        <TestMethod()>
        Public Sub HTTP_DownloadImage_ValidImageURL_ReturnsImage()
            'Arrange
            ' URL requests an 800x1200 JPEG thumbnail
            Dim imageURL As String = "https://thumb.theporndb.net/6aMjVmxotwfsJYYB1gFAb2-W1bc=/800x1200/smart/filters:sharpen():upscale()/scene%2F4d%2Fd3%2Fea%2F4b08887e02fb575a67f6be0afa60823%2Fbackground%2Fbg-fansdb-valsteele-onlyfans-legendary-club-orgy.jpg"

            'Act
            HTTP.StartDownloadImage(imageURL)
            ' Wait for the background thread to finish (max 30 seconds)
            Dim timeout As Integer = 30000
            Dim elapsed As Integer = 0
            Dim interval As Integer = 100
            Do While HTTP.IsDownloading() AndAlso elapsed < timeout
                Threading.Thread.Sleep(interval)
                elapsed += interval
            Loop

            'Assert
            Assert.IsFalse(HTTP.IsDownloading(), "Download did not finish within timeout")
            Assert.IsNotNull(HTTP.Image, "Image should not be Nothing after successful download")

            ' Image must have valid pixel dimensions
            Assert.IsTrue(HTTP.Image.Width > 0, "Image width should be greater than 0")
            Assert.IsTrue(HTTP.Image.Height > 0, "Image height should be greater than 0")

            ' The URL requests 800x1200 - server may return exact or proportional size
            Assert.IsTrue(HTTP.Image.Width <= 800, String.Format("Image width ({0}) should be <= 800", HTTP.Image.Width))
            Assert.IsTrue(HTTP.Image.Height <= 1200, String.Format("Image height ({0}) should be <= 1200", HTTP.Image.Height))

            ' Must be recognised as JPEG (URL ends in .jpg, content-type should be image/jpeg)
            Assert.IsTrue(HTTP.isJPG, "Image should be identified as JPEG")

            ' MemoryStream must contain actual image data (JPEG magic bytes: FF D8 FF)
            Dim streamBytes(2) As Byte
            HTTP.ms.Position = 0
            HTTP.ms.Read(streamBytes, 0, 3)
            Assert.AreEqual(CByte(&HFF), streamBytes(0), "Expected JPEG magic byte 1 (0xFF)")
            Assert.AreEqual(CByte(&HD8), streamBytes(1), "Expected JPEG magic byte 2 (0xD8)")
            Assert.AreEqual(CByte(&HFF), streamBytes(2), "Expected JPEG magic byte 3 (0xFF)")

            ' MemoryStream must contain substantial data (not just a tiny error response)
            Assert.IsTrue(HTTP.ms.Length > 10000, String.Format("MemoryStream should contain substantial image data, got {0} bytes", HTTP.ms.Length))
        End Sub

        <IntegrationTest>
        <TestMethod()>
        Public Sub HTTP_DownloadImage_InvalidURL_DoesNotCrash()
            'Arrange
            Dim imageURL As String = "http://this.url.does.not.exist.invalid/image.jpg"

            'Act
            HTTP.StartDownloadImage(imageURL)
            Dim timeout As Integer = 30000
            Dim elapsed As Integer = 0
            Dim interval As Integer = 100
            Do While HTTP.IsDownloading() AndAlso elapsed < timeout
                Threading.Thread.Sleep(interval)
                elapsed += interval
            Loop

            'Assert
            Assert.IsNull(HTTP.Image, "Image should be Nothing for an invalid URL")
        End Sub

        <IntegrationTest>
        <TestMethod()>
        Public Sub HTTP_DownloadImage_EmptyURL_DoesNotCrash()
            'Arrange
            ' Empty URL should be rejected by IsValidURL without making a request

            'Act
            HTTP.StartDownloadImage(String.Empty)
            Dim timeout As Integer = 5000
            Dim elapsed As Integer = 0
            Dim interval As Integer = 100
            Do While HTTP.IsDownloading() AndAlso elapsed < timeout
                Threading.Thread.Sleep(interval)
                elapsed += interval
            Loop

            'Assert
            Assert.IsNull(HTTP.Image, "Image should be Nothing for an empty URL")
        End Sub
        <IntegrationTest>
        <TestMethod()>
        Public Sub HTTP_DownloadZip()
            'Arrange

            'Act

            'Assert
            Assert.Inconclusive("Test not implemented")
        End Sub
        <UnitTest>
        <TestMethod()>
        Public Sub HTTP_PrepareProxy()
            'Arrange

            'Act

            'Assert
            Assert.Inconclusive("Test not implemented")
        End Sub
        <UnitTest>
        <TestMethod()>
        Public Sub HTTP_IsValidURL()
            'Arrange
            'The sourceValues should contain a String and Boolean - String is a URL, and bool to indicate whether it is indeed valid
            Dim sourceValues As New Dictionary(Of String, Boolean) From
                {
                    {String.Empty, False},
                    {"http://google.com", True},
                    {"google.ca", False},
                    {"http://google", True},
                    {"http://google.ca/", True},
                    {"http://www.google.ca/garbage", True},
                    {"http://i54.tinypic.com/27ybwqt.png", True},
                    {"http://i54.tinypic.com/27ybwqt.png/fdsa", True},
                    {"http://www.youtube.com/watch?v=SDnYMbYB-nU", True}
                }

            For Each pair As KeyValuePair(Of String, Boolean) In sourceValues
                'Act
                Dim result As Boolean = StringUtils.isValidURL(pair.Key)
                'Assert
                Assert.AreEqual(pair.Value, result, "Data tested was: '{0}' and was expecting '{1}', but received '{2}'", pair.Key, pair.Value, result)
            Next
        End Sub
        <UnitTest>
        <TestMethod()>
        Public Sub HTTP_IsValidURL_Nothing_parameter()
            'Arrange
            'Act
            Dim result As Boolean = StringUtils.isValidURL(Nothing)
            'Assert
            Assert.IsFalse(result, "Data tested was: 'Nothing' and was expecting 'False', but received '{0}'", result)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' dispose managed state (managed objects).
                    If HTTP IsNot Nothing Then HTTP.Dispose()
                End If

                ' free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' set large fields to null.
                HTTP = Nothing
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
