Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions
Imports EmberAPI
Imports NLog

Public Class VideoUtils

    Shared logger As Logger = NLog.LogManager.GetCurrentClassLogger()

    Public Shared Function GetVideoLengthInSeconds(videoPath As String) As Double
        Using ffmpeg As New Process()
            Try
                ffmpeg.StartInfo.FileName = Functions.GetFFMpeg
                ffmpeg.StartInfo.Arguments = String.Format("-ss 0 -i ""{0}"" -an -f rawvideo -vframes 1 -s 1280x720 -vcodec mjpeg -y ""{1}""", videoPath, Path.Combine(Master.TempPath, "frame.jpg"))
                ffmpeg.EnableRaisingEvents = False
                ffmpeg.StartInfo.UseShellExecute = False
                ffmpeg.StartInfo.CreateNoWindow = True
                ffmpeg.StartInfo.RedirectStandardOutput = True
                ffmpeg.StartInfo.RedirectStandardError = True
                ffmpeg.Start()
                ffmpeg.WaitForExit()
                Using err As StreamReader = ffmpeg.StandardError
                    Do
                        Dim s As String = err.ReadLine()
                        If s.Contains("Duration: ") Then
                            Dim sTime As String = Regex.Match(s, "Duration: (?<dur>.*?),").Groups("dur").ToString
                            If Not sTime = "N/A" Then
                                Dim ts As TimeSpan = CDate(CDate(String.Format("{0} {1}", DateTime.Today.ToString("d"), sTime))).Subtract(CDate(DateTime.Today))
                                Dim dSeconds As Double = ((ts.Hours * 60) + ts.Minutes) * 60 + ts.Seconds + (ts.Milliseconds / 1000)
                                '4, 28, 10, 11, 76
                                Return dSeconds
                            Else
                                logger.Error("ffmpeg coudn't detect duration for movie: {0}", videoPath)
                                Return Nothing
                            End If
                        End If
                    Loop While Not err.EndOfStream

                End Using
            Catch ex As Exception
                logger.Error(ex, "error occured during detect duration for movie: {0}", videoPath)
                Return Nothing
            Finally
                ffmpeg.Close()
            End Try
        End Using

    End Function

    Public Shared Function GrabFrameByPercentToImage(videoPath As String, percent As Double) As Image
        Dim l = GetVideoLengthInSeconds(videoPath)
        If l = Nothing Then
            Return Nothing
        End If
        Return GrabFrameBySecondsToImage(videoPath, (l * percent) / 100)
    End Function

    Shared Function GrabFrameBySecondsToImage(videoPath As String, seconds As Double) As Image
        Dim tmpPath = Path.Combine(Master.TempPath, "frame.jpg")
        Dim ffmpeg As New Process()
        Try

            ffmpeg.StartInfo.FileName = Functions.GetFFMpeg
            ffmpeg.StartInfo.Arguments = String.Format("-ss {0} -i ""{1}"" -vframes 1 -y ""{2}""", SecondsToString(seconds), videoPath, tmpPath)
            ffmpeg.EnableRaisingEvents = False
            ffmpeg.StartInfo.UseShellExecute = False
            ffmpeg.StartInfo.CreateNoWindow = True
            ffmpeg.StartInfo.RedirectStandardOutput = False
            ffmpeg.StartInfo.RedirectStandardError = False
            ffmpeg.Start()
            ffmpeg.WaitForExit()

            If File.Exists(tmpPath) Then
                Return PosterTemplateHelper.LoadImageFromFile(tmpPath)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            logger.Error(ex, "Cannot extract frame from movie: {0}", videoPath)
            Return Nothing
        Finally
            ffmpeg.Close()
            'File.Delete(tmpPath)
        End Try
    End Function

    Private Shared Function SecondsToString(seconds As Double) As String
        Dim nfi As NumberFormatInfo = New NumberFormatInfo()
        nfi.NumberDecimalSeparator = "."
        Return seconds.ToString("F3", nfi)
    End Function
End Class
