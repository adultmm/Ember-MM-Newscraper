' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports EmberAPI

Namespace EmberTests

    <TestClass()>
    Public Class Test_ScrapeData_Movie

        Private _savedModules As List(Of ModulesManager._externalScraperModuleClass_Data_Movie)

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

        Private Shared Sub AddFake(fake As FakeMovieDataScraper)
            ModulesManager.Instance.externalScrapersModules_Data_Movie.Add(
                New ModulesManager._externalScraperModuleClass_Data_Movie With {
                    .ProcessorModule = fake,
                    .ModuleOrder = 0
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
                Return New Interfaces.ModuleResult_Data_Movie With {
                    .Result = ResultToReturn,
                    .Cancelled = CancelledToReturn
                }
            End Function
        End Class

    End Class

End Namespace
