' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports EmberAPI

Namespace EmberTests

    <TestClass()>
    Public Class Test_ScrapeData_Movie

        Private _savedModules As List(Of ModulesManager._externalScraperModuleClass_Data_Movie)
        Private _scraperEventCount As Integer

        <TestInitialize>
        Public Sub TestInit()
            _savedModules = New List(Of ModulesManager._externalScraperModuleClass_Data_Movie)(ModulesManager.Instance.externalScrapersModules_Data_Movie)
            ModulesManager.Instance.externalScrapersModules_Data_Movie.Clear()
        End Sub

        <TestCleanup>
        Public Sub TestCleanup()
            ModulesManager.Instance.externalScrapersModules_Data_Movie.Clear()
            ModulesManager.Instance.externalScrapersModules_Data_Movie.AddRange(_savedModules)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ScrapeData_Movie_AdultStyleResult_HadResultsTrue_NotCancelled()
            AddFake(New FakeMovieDataScraper() With {
                .ResultToReturn = New MediaContainers.Movie With {
                    .Title = "Adult Title",
                    .UniqueIDs = New List(Of MediaContainers.Uniqueid) From {
                        New MediaContainers.Uniqueid With {.Type = "ae", .Value = "12345"}
                    }
                }
            })

            Dim hadResults As Boolean = False
            Dim cancelled = RunScrape(hadResults)

            Assert.IsFalse(cancelled)
            Assert.IsTrue(hadResults)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ScrapeData_Movie_NoResult_HadResultsFalse_NotCancelled_Issue160()
            AddFake(New FakeMovieDataScraper() With {.ResultToReturn = Nothing})

            Dim hadResults As Boolean = False
            Dim cancelled = RunScrape(hadResults)

            Assert.IsFalse(cancelled)
            Assert.IsFalse(hadResults)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ScrapeData_Movie_Cancelled_ReturnsTrue()
            AddFake(New FakeMovieDataScraper() With {.CancelledToReturn = True})

            Dim hadResults As Boolean = False
            Dim cancelled = RunScrape(hadResults)

            Assert.IsTrue(cancelled)
            Assert.IsFalse(hadResults)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ScrapeData_Movie_CancelledScraper_DoesNotLeakScraperEventHandler_Issue173()
            _scraperEventCount = 0
            AddHandler ModulesManager.Instance.ScraperEvent_Movie, AddressOf CountScraperEvent_Movie
            Try
                AddFake(New FakeMovieDataScraper() With {
                    .CancelledToReturn = True,
                    .RaiseScraperEvent = True
                })

                Dim hadResults As Boolean = False
                RunScrape(hadResults)
                RunScrape(hadResults)

                Assert.AreEqual(2, _scraperEventCount)
            Finally
                RemoveHandler ModulesManager.Instance.ScraperEvent_Movie, AddressOf CountScraperEvent_Movie
            End Try
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ScrapeData_Movie_SecondScraperCancelled_ReturnsTrueAndReportsEarlierResults()
            AddFake(New FakeMovieDataScraper() With {
                .ModuleOrderOverride = 0,
                .ResultToReturn = New MediaContainers.Movie With {.Title = "First"}
            })
            AddFake(New FakeMovieDataScraper() With {
                .ModuleOrderOverride = 1,
                .CancelledToReturn = True
            })

            Dim hadResults As Boolean = False
            Dim cancelled = RunScrape(hadResults)

            Assert.IsTrue(cancelled)
            Assert.IsTrue(hadResults)
        End Sub

        <UnitTest>
        <TestMethod()>
        Public Sub ScrapeImage_Movie_CancelledScraper_StopsChain_Issue173()
            Dim savedImageModules = New List(Of ModulesManager._externalScraperModuleClass_Image_Movie)(ModulesManager.Instance.externalScrapersModules_Image_Movie)
            ModulesManager.Instance.externalScrapersModules_Image_Movie.Clear()
            Try
                Dim first As New FakeMovieImageScraper() With {.ModuleOrderOverride = 0, .CancelledToReturn = True}
                Dim second As New FakeMovieImageScraper() With {.ModuleOrderOverride = 1}
                ModulesManager.Instance.externalScrapersModules_Image_Movie.Add(
                    New ModulesManager._externalScraperModuleClass_Image_Movie With {.ProcessorModule = first, .ModuleOrder = 0})
                ModulesManager.Instance.externalScrapersModules_Image_Movie.Add(
                    New ModulesManager._externalScraperModuleClass_Image_Movie With {.ProcessorModule = second, .ModuleOrder = 1})

                Dim db As New Database.DBElement(Enums.ContentType.Movie)
                db.IsOnline = True
                db.Filename = "C:\tmp\test-movie.mp4"
                db.Movie = New MediaContainers.Movie With {.Title = "Test Movie"}
                Dim images As New MediaContainers.SearchResultsContainer
                Dim modifiers As New Structures.ScrapeModifiers With {.MainPoster = True}

                Dim cancelled = ModulesManager.Instance.ScrapeImage_Movie(db, images, modifiers, False)

                Assert.IsTrue(cancelled)
                Assert.IsTrue(first.WasCalled)
                Assert.IsFalse(second.WasCalled)
            Finally
                ModulesManager.Instance.externalScrapersModules_Image_Movie.Clear()
                ModulesManager.Instance.externalScrapersModules_Image_Movie.AddRange(savedImageModules)
            End Try
        End Sub

        Private Sub CountScraperEvent_Movie(eType As Enums.ScraperEventType, parameter As Object)
            _scraperEventCount += 1
        End Sub

        Private Shared Sub AddFake(fake As FakeMovieDataScraper)
            ModulesManager.Instance.externalScrapersModules_Data_Movie.Add(
                New ModulesManager._externalScraperModuleClass_Data_Movie With {
                    .ProcessorModule = fake,
                    .ModuleOrder = If(fake.ModuleOrderOverride >= 0, fake.ModuleOrderOverride, 0)
                })
        End Sub

        Private Shared Function RunScrape(ByRef hadResults As Boolean) As Boolean
            Dim db As New Database.DBElement(Enums.ContentType.Movie)
            db.IsOnline = True
            db.Filename = "C:\tmp\test-movie.mp4"
            db.Movie = New MediaContainers.Movie With {.Title = "Test Movie"}
            Dim modifiers As New Structures.ScrapeModifiers With {.MainNFO = True}
            Dim options As New Structures.ScrapeOptions
            Return ModulesManager.Instance.ScrapeData_Movie(db, modifiers, Enums.ScrapeType.SelectedAsk, options, False, hadResults)
        End Function

        Private Class FakeMovieDataScraper
            Implements Interfaces.ScraperModule_Data_Movie

            Public Property ResultToReturn As MediaContainers.Movie
            Public Property CancelledToReturn As Boolean
            Public Property RaiseScraperEvent As Boolean
            Public Property ModuleOrderOverride As Integer = -1

            Public Event ModuleSettingsChanged() Implements Interfaces.ScraperModule_Data_Movie.ModuleSettingsChanged
            Public Event ScraperEvent(eType As Enums.ScraperEventType, Parameter As Object) Implements Interfaces.ScraperModule_Data_Movie.ScraperEvent
            Public Event ScraperSetupChanged(name As String, State As Boolean, difforder As Integer) Implements Interfaces.ScraperModule_Data_Movie.ScraperSetupChanged
            Public Event SetupNeedsRestart() Implements Interfaces.ScraperModule_Data_Movie.SetupNeedsRestart

            Public ReadOnly Property ModuleName As String Implements Interfaces.ScraperModule_Data_Movie.ModuleName
                Get
                    Return "FakeMovieData"
                End Get
            End Property

            Public ReadOnly Property ModuleVersion As String Implements Interfaces.ScraperModule_Data_Movie.ModuleVersion
                Get
                    Return "1.0"
                End Get
            End Property

            Public Property ScraperEnabled As Boolean Implements Interfaces.ScraperModule_Data_Movie.ScraperEnabled

            Public Sub New()
                ScraperEnabled = True
            End Sub

            Public Sub ScraperOrderChanged() Implements Interfaces.ScraperModule_Data_Movie.ScraperOrderChanged
            End Sub

            Public Function GetMovieStudio(ByRef DBMovie As Database.DBElement, ByRef sStudio As List(Of String)) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_Movie.GetMovieStudio
                Return New Interfaces.ModuleResult()
            End Function

            Public Function GetTMDBID(sIMDBID As String, ByRef sTMDBID As String) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_Movie.GetTMDBID
                Return New Interfaces.ModuleResult()
            End Function

            Public Sub Init(sAssemblyName As String) Implements Interfaces.ScraperModule_Data_Movie.Init
            End Sub

            Public Function InjectSetupScraper() As Containers.SettingsPanel Implements Interfaces.ScraperModule_Data_Movie.InjectSetupScraper
                Return Nothing
            End Function

            Public Sub SaveSetupScraper(DoDispose As Boolean) Implements Interfaces.ScraperModule_Data_Movie.SaveSetupScraper
            End Sub

            Public Function Scraper_Movie(ByRef oDBElement As Database.DBElement, ByRef ScrapeModifiers As Structures.ScrapeModifiers, ByRef ScrapeType As Enums.ScrapeType, ByRef ScrapeOptions As Structures.ScrapeOptions) As Interfaces.ModuleResult_Data_Movie Implements Interfaces.ScraperModule_Data_Movie.Scraper_Movie
                If RaiseScraperEvent Then
                    RaiseEvent ScraperEvent(Enums.ScraperEventType.NFOItem, "test")
                End If
                Return New Interfaces.ModuleResult_Data_Movie With {
                    .Result = ResultToReturn,
                    .Cancelled = CancelledToReturn
                }
            End Function
        End Class

        Private Class FakeMovieImageScraper
            Implements Interfaces.ScraperModule_Image_Movie

            Public Property CancelledToReturn As Boolean
            Public Property ModuleOrderOverride As Integer = -1
            Public Property WasCalled As Boolean

            Public Event ModuleSettingsChanged() Implements Interfaces.ScraperModule_Image_Movie.ModuleSettingsChanged
            Public Event ScraperEvent(eType As Enums.ScraperEventType, Parameter As Object) Implements Interfaces.ScraperModule_Image_Movie.ScraperEvent
            Public Event ScraperSetupChanged(name As String, State As Boolean, difforder As Integer) Implements Interfaces.ScraperModule_Image_Movie.ScraperSetupChanged
            Public Event SetupNeedsRestart() Implements Interfaces.ScraperModule_Image_Movie.SetupNeedsRestart
            Public Event ImagesDownloaded(Images As List(Of MediaContainers.Image)) Implements Interfaces.ScraperModule_Image_Movie.ImagesDownloaded
            Public Event ProgressUpdated(iPercent As Integer) Implements Interfaces.ScraperModule_Image_Movie.ProgressUpdated

            Public ReadOnly Property ModuleName As String Implements Interfaces.ScraperModule_Image_Movie.ModuleName
                Get
                    Return "FakeMovieImage"
                End Get
            End Property

            Public ReadOnly Property ModuleVersion As String Implements Interfaces.ScraperModule_Image_Movie.ModuleVersion
                Get
                    Return "1.0"
                End Get
            End Property

            Public Property ScraperEnabled As Boolean Implements Interfaces.ScraperModule_Image_Movie.ScraperEnabled

            Public Sub New()
                ScraperEnabled = True
            End Sub

            Public Sub ScraperOrderChanged() Implements Interfaces.ScraperModule_Image_Movie.ScraperOrderChanged
            End Sub

            Public Sub Init(sAssemblyName As String) Implements Interfaces.ScraperModule_Image_Movie.Init
            End Sub

            Public Function InjectSetupScraper() As Containers.SettingsPanel Implements Interfaces.ScraperModule_Image_Movie.InjectSetupScraper
                Return Nothing
            End Function

            Public Function QueryScraperCapabilities(cap As Enums.ModifierType) As Boolean Implements Interfaces.ScraperModule_Image_Movie.QueryScraperCapabilities
                Return cap = Enums.ModifierType.MainPoster
            End Function

            Public Sub SaveSetupScraper(DoDispose As Boolean) Implements Interfaces.ScraperModule_Image_Movie.SaveSetupScraper
            End Sub

            Public Function Scraper(ByRef DBMovie As Database.DBElement, ByRef ImagesContainer As MediaContainers.SearchResultsContainer, ByVal ScrapeModifiers As Structures.ScrapeModifiers) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Image_Movie.Scraper
                WasCalled = True
                Return New Interfaces.ModuleResult With {.Cancelled = CancelledToReturn}
            End Function
        End Class

    End Class

End Namespace
