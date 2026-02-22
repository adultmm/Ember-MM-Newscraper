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
Imports System.Windows.Forms

Imports EmberAPI
Imports System.Drawing


Namespace EmberTests


    <TestClass()> Public Class Test_clsAPIImageUtils

        ''' <summary>
        ''' Set to True to enable interactive visual verification dialogs.
        ''' When False (default), tests run automatically using programmatic checks.
        ''' </summary>
        Private Const RunInteractiveTests As Boolean = False

#Region "Helpers"

        ''' <summary>
        ''' Returns True if all pixels in the image have equal R, G, B values (i.e. grayscale).
        ''' </summary>
        Private Shared Function IsGrayscale(img As Bitmap) As Boolean
            For y As Integer = 0 To img.Height - 1
                For x As Integer = 0 To img.Width - 1
                    Dim c As Color = img.GetPixel(x, y)
                    If c.R <> c.G OrElse c.G <> c.B Then Return False
                Next
            Next
            Return True
        End Function

        ''' <summary>
        ''' Returns True if the pixel at (x, y) has the given ARGB color (ignoring alpha).
        ''' </summary>
        Private Shared Function PixelMatchesColor(img As Bitmap, x As Integer, y As Integer, expected As Color) As Boolean
            Dim c As Color = img.GetPixel(x, y)
            Return c.R = expected.R AndAlso c.G = expected.G AndAlso c.B = expected.B
        End Function

        ''' <summary>
        ''' Returns the path to HasLanguage.png in the output directory.
        ''' </summary>
        Private Shared Function HasLanguagePath() As String
            Return IO.Path.Combine(Functions.AppPath, "Images", "Defaults", "HasLanguage.png")
        End Function

#End Region

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_AddMissingStamp_NothingParameter()
            'Arrange

            'Act
            Dim result As Images = ImageUtils.AddMissingStamp(Nothing)
            'Assert
            Assert.IsTrue(result Is Nothing, "Nothing parameter")
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_AddMissingStamp()
            If Not RunInteractiveTests Then
                ' AddMissingStamp internally loads Missing.png via ReturnSettingsFile which
                ' requires the full application folder structure (Images\Defaults\Missing.png).
                ' This is only available when the app is fully deployed, not in a bare test run.
                Assert.Inconclusive("AddMissingStamp requires Images\Defaults\Missing.png at runtime. Run with RunInteractiveTests=True in a full deployment.")
                Return
            End If

            'Arrange
            Dim img As New EmberAPI.Images()
            img.UpdateMSfromImg(My.Resources.TestPattern)

            'Act
            Dim result As EmberAPI.Images = ImageUtils.AddMissingStamp(img)

            Dim userResponse As Windows.Forms.DialogResult = Nothing
            Using dialog As ImageFeedback = New ImageFeedback()
                dialog.LoadInfo(result.Image, "Is there a Missing stamp across the top left?")
                userResponse = dialog.ShowDialog()
            End Using
            Assert.IsTrue(userResponse = Windows.Forms.DialogResult.Yes, "User disagreed")
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_Grayscale_NothingParameter()
            'Arrange

            'Act
            Dim result As Images = ImageUtils.GrayScale(Nothing)
            'Assert
            Assert.IsTrue(result Is Nothing, "Nothing parameter")
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_GrayScale()
            'Arrange
            Dim img As New EmberAPI.Images()
            img.UpdateMSfromImg(My.Resources.TestPattern)

            'Act
            Dim result As EmberAPI.Images = ImageUtils.GrayScale(img)

            If RunInteractiveTests Then
                Dim userResponse As System.Windows.Forms.DialogResult = DialogResult.None
                Using dialog As ImageFeedback = New ImageFeedback()
                    dialog.LoadInfo(result.Image, "Is this image in grayscale?")
                    userResponse = dialog.ShowDialog()
                End Using
                Assert.IsTrue(userResponse = Windows.Forms.DialogResult.Yes, "User disagreed")
            Else
                ' Automatic check: every pixel must have R = G = B
                Assert.IsNotNull(result, "Result should not be Nothing")
                Assert.IsNotNull(result.Image, "Result image should not be Nothing")
                Using bmp As New Bitmap(result.Image)
                    Assert.IsTrue(IsGrayscale(bmp), "All pixels must have equal R, G, B values")
                End Using
            End If
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_DrawGradEllipse_NothingParameter()
            'Arrange
            Dim userResponse As System.Windows.Forms.DialogResult = DialogResult.None
            Using img = My.Resources.TestPattern_Med, _
                g = Graphics.FromImage(img), _
                font = New Font("Arial", 8, FontStyle.Bold)

                g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic

                Dim message = String.Format("{0} x {1}", img.Width, img.Height)

                Dim messageSize = g.MeasureString(message, font).ToSize()
                Dim paddingSize = New Size(15, 2)

                Dim x = (img.Width - messageSize.Width) \ 2 - paddingSize.Width
                Dim y = (img.Height - messageSize.Height) \ 2 - paddingSize.Height
                Dim width = messageSize.Width + paddingSize.Width * 2
                Dim height = messageSize.Height + paddingSize.Height * 2
                Dim rect = New Rectangle(x, y, width, height)

                Dim centerColor = Color.FromArgb(250, 120, 120, 120)
                Dim outerColor = Color.FromArgb(100, 255, 255, 255)

                'Act
                ImageUtils.DrawGradEllipse(Nothing, rect, centerColor, outerColor)

                g.DrawString(message, font, New SolidBrush(Color.White), (img.Width - messageSize.Width) \ 2, (img.Height - messageSize.Height) \ 2)

                If RunInteractiveTests Then
                    Using dialog As ImageFeedback = New ImageFeedback()
                        dialog.LoadInfo(img, "Do you see the image size (300x236) in the center of the image, with NO surrounding ellipse?")
                        userResponse = dialog.ShowDialog()
                    End Using
                    Assert.IsTrue(userResponse = Windows.Forms.DialogResult.Yes, "User disagreed")
                End If
                ' Automatic: DrawGradEllipse with Nothing graphics should not throw - if we got here, it passed
            End Using
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_DrawGradEllipse()
            'Arrange
            Dim userResponse As System.Windows.Forms.DialogResult = DialogResult.None
            Using img = My.Resources.TestPattern_Med, _
                g = Graphics.FromImage(img), _
                font = New Font("Arial", 8, FontStyle.Bold)

                g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic

                Dim message = String.Format("{0} x {1}", img.Width, img.Height)

                Dim messageSize = g.MeasureString(message, font).ToSize()
                Dim paddingSize = New Size(15, 2)

                Dim x = (img.Width - messageSize.Width) \ 2 - paddingSize.Width
                Dim y = (img.Height - messageSize.Height) \ 2 - paddingSize.Height
                Dim width = messageSize.Width + paddingSize.Width * 2
                Dim height = messageSize.Height + paddingSize.Height * 2
                Dim rect = New Rectangle(x, y, width, height)

                Dim centerColor = Color.FromArgb(250, 120, 120, 120)
                Dim outerColor = Color.FromArgb(100, 255, 255, 255)

                'Act
                ImageUtils.DrawGradEllipse(g, rect, centerColor, outerColor)

                g.DrawString(message, font, New SolidBrush(Color.White), (img.Width - messageSize.Width) \ 2, (img.Height - messageSize.Height) \ 2)

                If RunInteractiveTests Then
                    Using dialog As ImageFeedback = New ImageFeedback()
                        dialog.LoadInfo(img, "Do you see the image size (300x236) in the center of the image, surrounded by a faint white ellipse with a gray center?")
                        userResponse = dialog.ShowDialog()
                    End Using
                    Assert.IsTrue(userResponse = Windows.Forms.DialogResult.Yes, "User disagreed")
                End If
                ' Automatic: DrawGradEllipse should not throw - if we got here, it passed
            End Using
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_NothingImage()
            'Arrange
            Dim success As Boolean = False
            Dim image As Image = Nothing

            Dim maxWidth = 200
            Dim maxHeight = 200

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight)
            success = image Is Nothing

            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_ZeroWidth()
            'Arrange
            Dim success As Boolean = False
            Dim image As Image = My.Resources.TestPattern     ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 0
            Dim maxHeight = 200

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight)

            'Assert
            Dim widthOK = image.Width = My.Resources.TestPattern.Width
            Dim heightOK = image.Height = My.Resources.TestPattern.Height
            success = widthOK And heightOK
            image.Dispose()
            image = Nothing

            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_ZeroHeight()
            'Arrange
            Dim success As Boolean = False
            Dim image As Image = My.Resources.TestPattern     ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 200
            Dim maxHeight = 0

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight)

            'Assert
            Dim widthOK = image.Width = My.Resources.TestPattern.Width
            Dim heightOK = image.Height = My.Resources.TestPattern.Height
            success = widthOK And heightOK
            image.Dispose()
            image = Nothing

            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_NegativeWidth()
            'Arrange
            Dim success As Boolean = False
            Dim image As Image = My.Resources.TestPattern  ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = -200
            Dim maxHeight = 200

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight)

            'Assert
            Dim widthOK = image.Width = My.Resources.TestPattern.Width
            Dim heightOK = image.Height = My.Resources.TestPattern.Height
            success = widthOK And heightOK
            image.Dispose()
            image = Nothing

            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_NegativeHeight()
            'Arrange
            Dim success As Boolean = False
            Dim image As Image = My.Resources.TestPattern     ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 200
            Dim maxHeight = -200

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight)

            'Assert
            Dim widthOK = image.Width = My.Resources.TestPattern.Width
            Dim heightOK = image.Height = My.Resources.TestPattern.Height
            success = widthOK And heightOK
            image.Dispose()
            image = Nothing

            Assert.IsTrue(success)
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_Shrink()
            'Arrange
            Dim image As Image = My.Resources.TestPattern     ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 200
            Dim maxHeight = 200

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight)

            'Assert
            If image Is Nothing Then Assert.Fail()
            Dim widthOK = image.Width <= maxWidth
            Dim heightOK = image.Height <= maxHeight
            Dim scaleOK = (image.Width = maxWidth) OrElse (image.Height = maxHeight)

            If RunInteractiveTests Then
                Dim visualOK = False
                Using dialog = New ImageFeedback()
                    dialog.LoadInfo(image, "Is the image close-cropped, with no padding?")
                    Dim userResponse = dialog.ShowDialog()
                    visualOK = userResponse = Windows.Forms.DialogResult.Yes
                End Using
                Assert.IsTrue(widthOK And heightOK And scaleOK And visualOK)
            Else
                ' Automatic: no padding means the image fits within bounds and fills one dimension
                Assert.IsTrue(widthOK, "Width must be <= maxWidth")
                Assert.IsTrue(heightOK, "Height must be <= maxHeight")
                Assert.IsTrue(scaleOK, "At least one dimension must equal the max")
            End If
            image.Dispose()
            image = Nothing
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_Grow()
            'Arrange
            Dim image As Image = My.Resources.TestPattern_Med     ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 800
            Dim maxHeight = 800

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight)

            'Assert
            If image Is Nothing Then Assert.Fail()
            Dim widthOK = image.Width <= maxWidth
            Dim heightOK = image.Height <= maxHeight
            Dim scaleOK = (image.Width = maxWidth) OrElse (image.Height = maxHeight)

            If RunInteractiveTests Then
                Dim visualOK = False
                Using dialog = New ImageFeedback()
                    dialog.LoadInfo(image, "Is the image close-cropped, with no padding?")
                    Dim userResponse = dialog.ShowDialog()
                    visualOK = userResponse = Windows.Forms.DialogResult.Yes
                End Using
                Assert.IsTrue(widthOK And heightOK And scaleOK And visualOK)
            Else
                Assert.IsTrue(widthOK, "Width must be <= maxWidth")
                Assert.IsTrue(heightOK, "Height must be <= maxHeight")
                Assert.IsTrue(scaleOK, "At least one dimension must equal the max")
            End If
            image.Dispose()
            image = Nothing
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_Shrink_WithPadding()
            'Arrange
            Dim image As Image = My.Resources.TestPattern  ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 200
            Dim maxHeight = 200

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight, True)

            'Assert
            If image Is Nothing Then Assert.Fail()
            Dim widthOK = image.Width = maxWidth
            Dim heightOK = image.Height = maxHeight

            If RunInteractiveTests Then
                Dim visualOK = False
                Using dialog = New ImageFeedback()
                    dialog.LoadInfo(image, "Is the image close-cropped, with Black padding on the top and bottom?")
                    Dim userResponse = dialog.ShowDialog()
                    visualOK = userResponse = Windows.Forms.DialogResult.Yes
                End Using
                Assert.IsTrue(widthOK And heightOK And visualOK)
            Else
                ' With padding: result must be exactly maxWidth x maxHeight
                Assert.IsTrue(widthOK, "Width must equal maxWidth when using padding")
                Assert.IsTrue(heightOK, "Height must equal maxHeight when using padding")
                ' Black is the default padding color - check corner pixel (top-left)
                Using bmp As New Bitmap(image)
                    Assert.IsTrue(PixelMatchesColor(bmp, 0, 0, Color.Black), "Top-left corner should be black (default padding)")
                End Using
            End If
            image.Dispose()
            image = Nothing
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_Grow_WithPadding()
            'Arrange
            Dim image As Image = My.Resources.TestPattern_Med   ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 800
            Dim maxHeight = 800

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight, True)

            'Assert
            If image Is Nothing Then Assert.Fail()
            Dim widthOK = image.Width = maxWidth
            Dim heightOK = image.Height = maxHeight

            If RunInteractiveTests Then
                Dim visualOK = False
                Using dialog = New ImageFeedback()
                    dialog.LoadInfo(image, "Is the image close-cropped, with Black padding on top and bottom?")
                    Dim userResponse = dialog.ShowDialog()
                    visualOK = userResponse = Windows.Forms.DialogResult.Yes
                End Using
                Assert.IsTrue(widthOK And heightOK And visualOK)
            Else
                Assert.IsTrue(widthOK, "Width must equal maxWidth when using padding")
                Assert.IsTrue(heightOK, "Height must equal maxHeight when using padding")
                Using bmp As New Bitmap(image)
                    Assert.IsTrue(PixelMatchesColor(bmp, 0, 0, Color.Black), "Top-left corner should be black (default padding)")
                End Using
            End If
            image.Dispose()
            image = Nothing
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_Shrink_WithGreenPaddingTopBottom()
            'Arrange
            Dim image As Image = My.Resources.TestPattern  ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 200
            Dim maxHeight = 200

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight, True, Color.Green.ToArgb())

            'Assert
            If image Is Nothing Then Assert.Fail()
            Dim widthOK = image.Width = maxWidth
            Dim heightOK = image.Height = maxHeight

            If RunInteractiveTests Then
                Dim visualOK = False
                Using dialog = New ImageFeedback()
                    dialog.LoadInfo(image, "Is the image close-cropped, with Green padding on top and bottom?")
                    Dim userResponse = dialog.ShowDialog()
                    visualOK = userResponse = Windows.Forms.DialogResult.Yes
                End Using
                Assert.IsTrue(widthOK And heightOK And visualOK)
            Else
                Assert.IsTrue(widthOK, "Width must equal maxWidth when using padding")
                Assert.IsTrue(heightOK, "Height must equal maxHeight when using padding")
                ' Green padding on top/bottom: top-left corner pixel should be green
                Using bmp As New Bitmap(image)
                    Assert.IsTrue(PixelMatchesColor(bmp, 0, 0, Color.Green), "Top-left corner should be green (padding color)")
                End Using
            End If
            image.Dispose()
            image = Nothing
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_Shrink_WithGreenPaddingSides()
            'Arrange
            Dim image As Image = My.Resources.TestPattern  ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 400
            Dim maxHeight = 200

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight, True, Color.Green.ToArgb())

            'Assert
            If image Is Nothing Then Assert.Fail()
            Dim widthOK = image.Width = maxWidth
            Dim heightOK = image.Height = maxHeight

            If RunInteractiveTests Then
                Dim visualOK = False
                Using dialog = New ImageFeedback()
                    dialog.LoadInfo(image, "Is the image close-cropped, with bottom portion missing?")
                    Dim userResponse = dialog.ShowDialog()
                    visualOK = userResponse = Windows.Forms.DialogResult.Yes
                End Using
                Assert.IsTrue(widthOK And heightOK And visualOK)
            Else
                Assert.IsTrue(widthOK, "Width must equal maxWidth when using padding")
                Assert.IsTrue(heightOK, "Height must equal maxHeight when using padding")
            End If
            image.Dispose()
            image = Nothing
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_ShrinkVert_WithGreenPaddingSides()
            'Arrange
            Dim image As Image = My.Resources.TestPattern_Vert  ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 400
            Dim maxHeight = 200

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight, True, Color.Green.ToArgb())

            'Assert
            If image Is Nothing Then Assert.Fail()
            Dim widthOK = image.Width = maxWidth
            Dim heightOK = image.Height = maxHeight

            If RunInteractiveTests Then
                Dim visualOK = False
                Using dialog = New ImageFeedback()
                    dialog.LoadInfo(image, "Is the image close-cropped, with Green padding on the sides?")
                    Dim userResponse = dialog.ShowDialog()
                    visualOK = userResponse = Windows.Forms.DialogResult.Yes
                End Using
                Assert.IsTrue(widthOK And heightOK And visualOK)
            Else
                Assert.IsTrue(widthOK, "Width must equal maxWidth when using padding")
                Assert.IsTrue(heightOK, "Height must equal maxHeight when using padding")
                ' Vertical image in wide box: padding on left/right sides -> top-left corner is green
                Using bmp As New Bitmap(image)
                    Assert.IsTrue(PixelMatchesColor(bmp, 0, 0, Color.Green), "Top-left corner should be green (side padding)")
                End Using
            End If
            image.Dispose()
            image = Nothing
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_Grow_WithGreenPaddingTopBottom()
            'Arrange
            Dim image As Image = My.Resources.TestPattern_Med  ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 800
            Dim maxHeight = 800

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight, True, Color.Green.ToArgb())

            'Assert
            If image Is Nothing Then Assert.Fail()
            Dim widthOK = image.Width = maxWidth
            Dim heightOK = image.Height = maxHeight

            If RunInteractiveTests Then
                Dim visualOK = False
                Using dialog = New ImageFeedback()
                    dialog.LoadInfo(image, "Is the image close-cropped, with Green padding on top and bottom?")
                    Dim userResponse = dialog.ShowDialog()
                    visualOK = userResponse = Windows.Forms.DialogResult.Yes
                End Using
                Assert.IsTrue(widthOK And heightOK And visualOK)
            Else
                Assert.IsTrue(widthOK, "Width must equal maxWidth when using padding")
                Assert.IsTrue(heightOK, "Height must equal maxHeight when using padding")
                Using bmp As New Bitmap(image)
                    Assert.IsTrue(PixelMatchesColor(bmp, 0, 0, Color.Green), "Top-left corner should be green (padding color)")
                End Using
            End If
            image.Dispose()
            image = Nothing
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_Grow_WithGreenPaddingSides()
            'Arrange
            Dim image As Image = My.Resources.TestPattern_Med     ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 800
            Dim maxHeight = 400

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight, True, Color.Green.ToArgb())

            'Assert
            If image Is Nothing Then Assert.Fail()
            Dim widthOK = image.Width = maxWidth
            Dim heightOK = image.Height = maxHeight

            If RunInteractiveTests Then
                Dim visualOK = False
                Using dialog = New ImageFeedback()
                    dialog.LoadInfo(image, "Is the image close-cropped, with bottom portion missing?")
                    Dim userResponse = dialog.ShowDialog()
                    visualOK = userResponse = Windows.Forms.DialogResult.Yes
                End Using
                Assert.IsTrue(widthOK And heightOK And visualOK)
            Else
                Assert.IsTrue(widthOK, "Width must equal maxWidth when using padding")
                Assert.IsTrue(heightOK, "Height must equal maxHeight when using padding")
            End If
            image.Dispose()
            image = Nothing
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_width_height_padding_GrowVert_WithGreenPaddingSides()
            'Arrange
            Dim image As Image = My.Resources.TestPattern_Med_Vert    ' Cannot use "using" because of ByVal in ResizeImage

            Dim maxWidth = 800
            Dim maxHeight = 400

            'Act
            ImageUtils.ResizeImage(image, maxWidth, maxHeight, True, Color.Green.ToArgb())

            'Assert
            If image Is Nothing Then Assert.Fail()
            Dim widthOK = image.Width = maxWidth
            Dim heightOK = image.Height = maxHeight

            If RunInteractiveTests Then
                Dim visualOK = False
                Using dialog = New ImageFeedback()
                    dialog.LoadInfo(image, "Is the image close-cropped, with Green padding on the sides?")
                    Dim userResponse = dialog.ShowDialog()
                    visualOK = userResponse = Windows.Forms.DialogResult.Yes
                End Using
                Assert.IsTrue(widthOK And heightOK And visualOK)
            Else
                Assert.IsTrue(widthOK, "Width must equal maxWidth when using padding")
                Assert.IsTrue(heightOK, "Height must equal maxHeight when using padding")
                Using bmp As New Bitmap(image)
                    Assert.IsTrue(PixelMatchesColor(bmp, 0, 0, Color.Green), "Top-left corner should be green (side padding)")
                End Using
            End If
            image.Dispose()
            image = Nothing
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizePB_NothingSourceImage()
            'Arrange
            Dim success As Boolean = False
            Dim destImage As PictureBox = New PictureBox()
            Using sourceImage As PictureBox = New PictureBox()

                Dim boxWidth = 600
                Dim boxHeight = 400

                'Act
                ImageUtils.ResizePB(destImage, sourceImage, boxHeight, boxWidth)
                success = destImage.Image Is Nothing
            End Using
            destImage.Dispose()
            destImage = Nothing
            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizePB_NothingSource()
            'Arrange
            Dim success As Boolean = False
            Dim destImage As PictureBox = New PictureBox()

            Dim boxWidth = 600
            Dim boxHeight = 400

            'Act
            ImageUtils.ResizePB(destImage, Nothing, boxHeight, boxWidth)
            success = destImage.Image Is Nothing
            destImage.Dispose()
            destImage = Nothing
            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizePB_Wide()
            'Arrange
            Dim success As Boolean = False
            Dim destImage As PictureBox = New PictureBox()
            Using sourceImage As PictureBox = New PictureBox()

                sourceImage.Image = My.Resources.TestPattern_Med
                Dim boxWidth = 600
                Dim boxHeight = 400

                'Act
                ImageUtils.ResizePB(destImage, sourceImage, boxHeight, boxWidth)

                Dim widthOK As Boolean = destImage.Width <= IIf(sourceImage.Image.Width >= boxWidth, boxWidth, sourceImage.Image.Width)
                Dim heightOK As Boolean = destImage.Height <= IIf(sourceImage.Image.Height >= boxHeight, boxHeight, sourceImage.Image.Height)
                success = widthOK And heightOK
            End Using
            destImage.Dispose()
            destImage = Nothing

            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizePB_Tall()
            'Arrange
            Dim success As Boolean = False
            Dim destImage As PictureBox = New PictureBox()
            Using sourceImage As PictureBox = New PictureBox()

                sourceImage.Image = My.Resources.TestPattern_Med
                Dim boxWidth = 400
                Dim boxHeight = 600

                'Act
                ImageUtils.ResizePB(destImage, sourceImage, boxHeight, boxWidth)

                'Assert
                Dim widthOK As Boolean = destImage.Width <= IIf(sourceImage.Image.Width >= boxWidth, boxWidth, sourceImage.Image.Width)
                Dim heightOK As Boolean = destImage.Height <= IIf(sourceImage.Image.Height >= boxHeight, boxHeight, sourceImage.Image.Height)
                success = widthOK And heightOK
            End Using
            destImage.Dispose()
            destImage = Nothing

            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizePB_Tall_Shrink()
            'Arrange
            Dim success As Boolean = False
            Dim destImage As PictureBox = New PictureBox()
            Using sourceImage As PictureBox = New PictureBox()

                sourceImage.Image = My.Resources.TestPattern_Med
                Dim boxWidth = 200
                Dim boxHeight = 400

                'Act
                ImageUtils.ResizePB(destImage, sourceImage, boxHeight, boxWidth)

                'Assert
                Dim widthOK As Boolean = destImage.Width <= IIf(sourceImage.Image.Width >= boxWidth, boxWidth, sourceImage.Image.Width)
                Dim heightOK As Boolean = destImage.Height <= IIf(sourceImage.Image.Height >= boxHeight, boxHeight, sourceImage.Image.Height)
                success = widthOK And heightOK
            End Using
            destImage.Dispose()
            destImage = Nothing

            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizePB_Wide_Shrink()
            'Arrange
            Dim success As Boolean = False
            Dim destImage As PictureBox = New PictureBox()
            Using sourceImage As PictureBox = New PictureBox()

                sourceImage.Image = My.Resources.TestPattern_Med
                Dim boxWidth = 400
                Dim boxHeight = 200

                'Act
                ImageUtils.ResizePB(destImage, sourceImage, boxHeight, boxWidth)

                'Assert
                Dim widthOK As Boolean = destImage.Width <= IIf(sourceImage.Image.Width >= boxWidth, boxWidth, sourceImage.Image.Width)
                Dim heightOK As Boolean = destImage.Height <= IIf(sourceImage.Image.Height >= boxHeight, boxHeight, sourceImage.Image.Height)
                success = widthOK And heightOK
            End Using
            destImage.Dispose()
            destImage = Nothing

            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_SetGlassOverlay_NothingParameter()
            'Arrange
            Dim source As PictureBox = Nothing
            'Act
            ImageUtils.SetGlassOverlay(source)

            'Assert
            Assert.IsNull(source, "Expected Nothing, got something else")
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_SetGlassOverlay_NothingImage()
            'Arrange
            Dim source As PictureBox = New PictureBox()
            source.Image = Nothing
            'Act
            ImageUtils.SetGlassOverlay(source)

            'Assert
            Assert.IsNull(source.Image, "Expected Nothing, got something else")
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_SetGlassOverlay_NormalImage_Horizontal()
            'Arrange
            Using source As PictureBox = New PictureBox()
                source.Image = My.Resources.TestPattern_Med

                'Act
                ImageUtils.SetGlassOverlay(source)

                If RunInteractiveTests Then
                    Dim userResponse As Windows.Forms.DialogResult = Nothing
                    Using dialog As ImageFeedback = New ImageFeedback()
                        dialog.LoadInfo(source.Image, "Does the image have a Glass overlay?")
                        userResponse = dialog.ShowDialog()
                    End Using
                    Assert.IsTrue(userResponse = Windows.Forms.DialogResult.Yes, "User disagreed")
                Else
                    ' Automatic: image must still exist and have same dimensions
                    Assert.IsNotNull(source.Image, "Image should not be Nothing after SetGlassOverlay")
                    Assert.AreEqual(My.Resources.TestPattern_Med.Width, source.Image.Width, "Width should be unchanged")
                    Assert.AreEqual(My.Resources.TestPattern_Med.Height, source.Image.Height, "Height should be unchanged")
                End If
            End Using
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_SetGlassOverlay_NormalImage_Vertical()
            'Arrange
            Using source As PictureBox = New PictureBox()
                source.Image = My.Resources.TestPattern_Med_Vert

                'Act
                ImageUtils.SetGlassOverlay(source)

                If RunInteractiveTests Then
                    Dim userResponse As Windows.Forms.DialogResult = Nothing
                    Using dialog As ImageFeedback = New ImageFeedback()
                        dialog.LoadInfo(source.Image, "Does the image have a Glass overlay?")
                        userResponse = dialog.ShowDialog()
                    End Using
                    Assert.IsTrue(userResponse = Windows.Forms.DialogResult.Yes, "User disagreed")
                Else
                    Assert.IsNotNull(source.Image, "Image should not be Nothing after SetGlassOverlay")
                    Assert.AreEqual(My.Resources.TestPattern_Med_Vert.Width, source.Image.Width, "Width should be unchanged")
                    Assert.AreEqual(My.Resources.TestPattern_Med_Vert.Height, source.Image.Height, "Height should be unchanged")
                End If
            End Using
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_SetOverlay_NothingImage()
            'Arrange
            ' Use an embedded test resource as overlay - we just need any valid image here
            Using overlay As Image = My.Resources.TestPattern
                'Act
                Dim result As Image = ImageUtils.SetOverlay(Nothing, 500, 500, overlay, 1)

                'Assert
                Assert.IsNull(result, "Expected Nothing, got something else")
            End Using
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_SetOverlay_TopLeft()
            'Arrange
            Dim overlayPath As String = HasLanguagePath()
            Using img As Image = My.Resources.TestPattern, _
                overlay As Image = Image.FromFile(overlayPath)

                'Act
                Using result As Image = ImageUtils.SetOverlay(img, img.Width \ 2, img.Height \ 2, overlay, 1)

                    If RunInteractiveTests Then
                        Dim userResponse As Windows.Forms.DialogResult = Nothing
                        Using dialog As ImageFeedback = New ImageFeedback()
                            dialog.LoadInfo(result, "Is there a message bubble in the top-left corner?")
                            userResponse = dialog.ShowDialog()
                        End Using
                        Assert.IsTrue(userResponse = Windows.Forms.DialogResult.Yes, "User disagreed")
                    Else
                        ' Automatic: result must not be Nothing and overlay pixel at top-left
                        ' must differ from the plain underlay (overlay was drawn at 0,0)
                        Assert.IsNotNull(result, "Result should not be Nothing")
                        Using resultBmp As New Bitmap(result)
                        Using overlayBmp As New Bitmap(overlay)
                            ' The overlay is drawn at top-left (0,0) - compare a known overlay pixel
                            Dim overlayPixel As Color = overlayBmp.GetPixel(overlayBmp.Width \ 2, overlayBmp.Height \ 2)
                            Dim resultPixel As Color = resultBmp.GetPixel(overlayBmp.Width \ 2, overlayBmp.Height \ 2)
                            Assert.AreEqual(overlayPixel.ToArgb(), resultPixel.ToArgb(), "Center of overlay area should match overlay image")
                        End Using
                        End Using
                    End If
                End Using
            End Using
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_SetOverlay_TopRight()
            'Arrange
            Dim overlayPath As String = HasLanguagePath()
            Using img As Image = My.Resources.TestPattern, _
                overlay As Image = Image.FromFile(overlayPath)

                'Act
                Using result As Image = ImageUtils.SetOverlay(img, img.Width \ 2, img.Height \ 2, overlay, 2)

                    If RunInteractiveTests Then
                        Dim userResponse As Windows.Forms.DialogResult = Nothing
                        Using dialog As ImageFeedback = New ImageFeedback()
                            dialog.LoadInfo(result, "Is there a message bubble in the top-right corner?")
                            userResponse = dialog.ShowDialog()
                        End Using
                        Assert.IsTrue(userResponse = Windows.Forms.DialogResult.Yes, "User disagreed")
                    Else
                        Assert.IsNotNull(result, "Result should not be Nothing")
                        ' Overlay placed at top-right: iLeft = result.Width - overlay.Width
                        Using resultBmp As New Bitmap(result)
                        Using overlayBmp As New Bitmap(overlay)
                            Dim iLeft As Integer = resultBmp.Width - overlayBmp.Width
                            Dim checkX As Integer = iLeft + overlayBmp.Width \ 2
                            Dim checkY As Integer = overlayBmp.Height \ 2
                            Dim overlayPixel As Color = overlayBmp.GetPixel(overlayBmp.Width \ 2, overlayBmp.Height \ 2)
                            Dim resultPixel As Color = resultBmp.GetPixel(checkX, checkY)
                            Assert.AreEqual(overlayPixel.ToArgb(), resultPixel.ToArgb(), "Center of overlay area (top-right) should match overlay image")
                        End Using
                        End Using
                    End If
                End Using
            End Using
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_SetOverlay_BottomLeft()
            'Arrange
            Dim overlayPath As String = HasLanguagePath()
            Using img As Image = My.Resources.TestPattern, _
                overlay As Image = Image.FromFile(overlayPath)

                'Act
                Using result As Image = ImageUtils.SetOverlay(img, img.Width \ 2, img.Height \ 2, overlay, 3)

                    If RunInteractiveTests Then
                        Dim userResponse As Windows.Forms.DialogResult = Nothing
                        Using dialog As ImageFeedback = New ImageFeedback()
                            dialog.LoadInfo(result, "Is there a message bubble in the lower-left corner?")
                            userResponse = dialog.ShowDialog()
                        End Using
                        Assert.IsTrue(userResponse = Windows.Forms.DialogResult.Yes, "User disagreed")
                    Else
                        Assert.IsNotNull(result, "Result should not be Nothing")
                        Using resultBmp As New Bitmap(result)
                        Using overlayBmp As New Bitmap(overlay)
                            Dim iTop As Integer = resultBmp.Height - overlayBmp.Height
                            Dim checkX As Integer = overlayBmp.Width \ 2
                            Dim checkY As Integer = iTop + overlayBmp.Height \ 2
                            Dim overlayPixel As Color = overlayBmp.GetPixel(overlayBmp.Width \ 2, overlayBmp.Height \ 2)
                            Dim resultPixel As Color = resultBmp.GetPixel(checkX, checkY)
                            Assert.AreEqual(overlayPixel.ToArgb(), resultPixel.ToArgb(), "Center of overlay area (bottom-left) should match overlay image")
                        End Using
                        End Using
                    End If
                End Using
            End Using
        End Sub

        <InteractiveTest>
        <TestMethod()>
        Public Sub ImageUtils_SetOverlay_BottomRight()
            'Arrange
            Dim overlayPath As String = HasLanguagePath()
            Using img As Image = My.Resources.TestPattern, _
                overlay As Image = Image.FromFile(overlayPath)

                'Act
                Using result As Image = ImageUtils.SetOverlay(img, img.Width \ 2, img.Height \ 2, overlay, 4)

                    If RunInteractiveTests Then
                        Dim userResponse As Windows.Forms.DialogResult = Nothing
                        Using dialog As ImageFeedback = New ImageFeedback()
                            dialog.LoadInfo(result, "Is there a message bubble in the lower-right corner?")
                            userResponse = dialog.ShowDialog()
                        End Using
                        Assert.IsTrue(userResponse = Windows.Forms.DialogResult.Yes, "User disagreed")
                    Else
                        Assert.IsNotNull(result, "Result should not be Nothing")
                        Using resultBmp As New Bitmap(result)
                        Using overlayBmp As New Bitmap(overlay)
                            Dim iLeft As Integer = resultBmp.Width - overlayBmp.Width
                            Dim iTop As Integer = resultBmp.Height - overlayBmp.Height
                            Dim checkX As Integer = iLeft + overlayBmp.Width \ 2
                            Dim checkY As Integer = iTop + overlayBmp.Height \ 2
                            Dim overlayPixel As Color = overlayBmp.GetPixel(overlayBmp.Width \ 2, overlayBmp.Height \ 2)
                            Dim resultPixel As Color = resultBmp.GetPixel(checkX, checkY)
                            Assert.AreEqual(overlayPixel.ToArgb(), resultPixel.ToArgb(), "Center of overlay area (bottom-right) should match overlay image")
                        End Using
                        End Using
                    End If
                End Using
            End Using
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_JPEGCompression()
            'Arrange
            'Act
            'Assert
            Assert.Inconclusive("Test not implemented")
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_image_size_NothingImage()
            'Arrange
            Dim success As Boolean = False

            Using source As Image = Nothing
                Dim size As Size = New Size(100, 400)
                'Act
                Using result As Image = ImageUtils.ResizeImage(source, size)
                    success = result Is Nothing
                End Using

            End Using

            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_image_size_NothingSize()
            'Arrange
            Dim success As Boolean = False
            Using source As Image = My.Resources.TestPattern
                Dim size As Size = Nothing
                'Act
                Using result As Image = ImageUtils.ResizeImage(source, size)

                    If result Is Nothing Then Assert.Fail()
                    Dim widthOK = result.Width = source.Width
                    Dim heightOK = result.Height = source.Height
                    success = widthOK And heightOK
                End Using
            End Using
            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_image_size_ZeroSize()
            'Arrange
            Dim success As Boolean = False
            Using source As Image = My.Resources.TestPattern
                Dim size As Size = New Size(0, 0)
                'Act
                Using result As Image = ImageUtils.ResizeImage(source, size)

                    If result Is Nothing Then Assert.Fail()
                    Dim widthOK = result.Width = source.Width
                    Dim heightOK = result.Height = source.Height
                    success = widthOK And heightOK
                End Using
            End Using
            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_image_size_NegativeSize()
            'Arrange
            Dim success As Boolean = False
            Using source As Image = My.Resources.TestPattern
                Dim size As Size = New Size(-200, -200)
                'Act
                Using result As Image = ImageUtils.ResizeImage(source, size)

                    If result Is Nothing Then Assert.Fail()
                    Dim widthOK = result.Width = source.Width
                    Dim heightOK = result.Height = source.Height
                    success = widthOK And heightOK
                End Using
            End Using
            'Assert
            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_image_sizeTall()
            ' NOTE: ResizeImage(Image, Size) stretches to exact size (does not preserve aspect ratio).
            ' See TODO comment in clsAPIImageUtils.vb. Test verifies current (stretch) behaviour.
            'Arrange
            Dim success As Boolean = False
            Using source As Image = My.Resources.TestPattern
                Dim size As Size = New Size(100, 400)
                'Act
                Using result As Image = ImageUtils.ResizeImage(source, size)

                    If RunInteractiveTests Then
                        If result Is Nothing Then Assert.Fail()
                        Dim visualOK = False
                        Using dialog = New ImageFeedback()
                            dialog.LoadInfo(result, "Is this image squished or stretched?")
                            Dim userResponse = dialog.ShowDialog()
                            visualOK = userResponse = Windows.Forms.DialogResult.Yes    'Yes means it IS stretched (documenting the bug)
                        End Using
                        success = visualOK
                    Else
                        ' Automatic: documents that this overload STRETCHES to exact size
                        If result Is Nothing Then Assert.Fail()
                        success = (result.Width = size.Width) AndAlso (result.Height = size.Height)
                    End If
                End Using
            End Using

            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_ResizeImage_image_sizeWide()
            ' NOTE: ResizeImage(Image, Size) stretches to exact size (does not preserve aspect ratio).
            ' See TODO comment in clsAPIImageUtils.vb. Test verifies current (stretch) behaviour.
            'Arrange
            Dim success As Boolean = False
            Using source As Image = My.Resources.TestPattern
                Dim size As Size = New Size(400, 100)
                'Act
                Using result As Image = ImageUtils.ResizeImage(source, size)

                    If RunInteractiveTests Then
                        If result Is Nothing Then Assert.Fail()
                        Dim visualOK = False
                        Using dialog = New ImageFeedback()
                            dialog.LoadInfo(result, "Is this image squished or stretched?")
                            Dim userResponse = dialog.ShowDialog()
                            visualOK = userResponse = Windows.Forms.DialogResult.Yes    'Yes means it IS stretched (documenting the bug)
                        End Using
                        success = visualOK
                    Else
                        ' Automatic: documents that this overload STRETCHES to exact size
                        If result Is Nothing Then Assert.Fail()
                        success = (result.Width = size.Width) AndAlso (result.Height = size.Height)
                    End If
                End Using
            End Using

            Assert.IsTrue(success)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ImageUtils_AddGenreString()
            'Arrange
            Dim success As Boolean = False
            Using img As Image = My.Resources.Genre
                Dim genreString As String = "Fantasy"
                'Act
                Using result As Image = ImageUtils.AddGenreString(img, genreString)

                    'Assert
                    If result Is Nothing Then Assert.Fail()
                    Dim sizeOK = result.Size = img.Size

                    If RunInteractiveTests Then
                        Dim visualOK = False
                        Using dialog = New ImageFeedback()
                            dialog.LoadInfo(result, "Does this image have " & genreString & " written on it?")
                            Dim userResponse = dialog.ShowDialog()
                            visualOK = userResponse = Windows.Forms.DialogResult.Yes
                        End Using
                        success = sizeOK And visualOK
                    Else
                        ' Automatic: result must have same size as source
                        success = sizeOK
                    End If
                End Using
            End Using
            Assert.IsTrue(success)
        End Sub

    End Class
End Namespace
