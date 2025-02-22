using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.PreCom.Interfaces;
using Drogecode.Knrm.Oefenrooster.PreCom.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Drogecode.Knrm.Oefenrooster.TestPreCom.Tests;

public class PreComWorkerTests
{
    private readonly ILogger _logger;

    public PreComWorkerTests(ILoggerProvider loggerProvider)
    {
        _logger = loggerProvider.CreateLogger("PreComWorkerTests");
    }

    [Fact]
    public async Task NextHourTest()
    {
        var mockClient = MockClient();
        var worker = new PreComWorker(mockClient, _logger);
        var result = await worker.Work(NextRunMode.NextHour);
        Assert.NotNull(result.Problems);
        result.Problems.Trim().Should().Be("Voor aankomend uur hebben we nog een schipper nodig opstapper nodig algemeen nodig");
    }

    [Fact]
    public async Task TodayTomorrowTest()
    {
        var mockClient = MockClient();
        var worker = new PreComWorker(mockClient, _logger);
        var result = await worker.Work(NextRunMode.TodayTomorrow);
        Assert.NotNull(result.Problems);
        result.Problems.Trim().Should().Be("We hebben morgen nog gaten in het rooster");
    }

    [Fact]
    public async Task NextWeekTest()
    {
        var mockClient = MockClient();
        var worker = new PreComWorker(mockClient, _logger);
        var result = await worker.Work(NextRunMode.NextWeek);
        Assert.NotNull(result.Problems);
        result.Problems.Trim().Should().Be(@"{0}
Schipper
van 8 tot 16

Opstapper
van 2 tot 16

Aankomend opstapper
van 0 tot 2
van 8 tot 16
van 22 tot 24

{1}
Schipper
van 8 tot 16

Opstapper
van 2 tot 16

Aankomend opstapper
van 0 tot 2
van 8 tot 16
van 22 tot 24

{2}
Schipper
van 8 tot 16

Opstapper
van 2 tot 16

Aankomend opstapper
van 0 tot 2
van 8 tot 16
van 22 tot 24

{3}
Schipper
van 8 tot 16

Opstapper
van 2 tot 16

Aankomend opstapper
van 0 tot 2
van 8 tot 16
van 22 tot 24

{4}
Schipper
van 8 tot 16

Opstapper
van 2 tot 16

Aankomend opstapper
van 0 tot 2
van 8 tot 16
van 22 tot 24");
    }

    private IPreComClient MockClient()
    {
        var mockClient = Substitute.For<IPreComClient>();
        mockClient.GetAllUserGroups().Returns([new Group { GroupID = 1201 }]);


        mockClient.GetAllFunctions(1201, DateTime.Today).Returns(GetGroup(DateTime.Today));
        mockClient.GetAllFunctions(1201, DateTime.Today.AddDays(1)).Returns(GetGroup(DateTime.Today.AddDays(1)));
        mockClient.GetAllFunctions(1201, DateTime.Today.AddDays(2)).Returns(GetGroup(DateTime.Today.AddDays(2)));
        mockClient.GetAllFunctions(1201, DateTime.Today.AddDays(3)).Returns(GetGroup(DateTime.Today.AddDays(3)));
        mockClient.GetAllFunctions(1201, DateTime.Today.AddDays(4)).Returns(GetGroup(DateTime.Today.AddDays(4)));
        mockClient.GetAllFunctions(1201, DateTime.Today.AddDays(5)).Returns(GetGroup(DateTime.Today.AddDays(5)));
        mockClient.GetAllFunctions(1201, DateTime.Today.AddDays(6)).Returns(GetGroup(DateTime.Today.AddDays(6)));
        mockClient.GetOccupancyLevels(1201, DateTime.Today, DateTime.Today.AddDays(7)).Returns(new Dictionary<DateTime, int>()
        {
            {
                DateTime.Today, -1
            },
            {
                DateTime.Today.AddDays(1), -1
            },
            {
                DateTime.Today.AddDays(2), -1
            },
            {
                DateTime.Today.AddDays(3), 1
            },
            {
                DateTime.Today.AddDays(4), -1
            },
            {
                DateTime.Today.AddDays(5), 1
            },
            {
                DateTime.Today.AddDays(6), -1
            }
        });
        return mockClient;
    }

    public Dictionary<string, bool?> GetDefaultHoursDictionary(bool hour0, bool hour2, bool hour8, bool hour16, bool hour22)
    {
        return new Dictionary<string, bool?>()
        {
            {
                "Hour0", hour0
            },
            {
                "Hour1", hour0
            },
            {
                "Hour2", hour2
            },
            {
                "Hour3", hour2
            },
            {
                "Hour4", hour2
            },
            {
                "Hour5", hour2
            },
            {
                "Hour6", hour2
            },
            {
                "Hour7", hour2
            },
            {
                "Hour8", hour8
            },
            {
                "Hour9", hour8
            },
            {
                "Hour10", hour8
            },
            {
                "Hour11", hour8
            },
            {
                "Hour12", hour8
            },
            {
                "Hour13", hour8
            },
            {
                "Hour14", hour8
            },
            {
                "Hour15", hour8
            },
            {
                "Hour16", hour16
            },
            {
                "Hour17", hour16
            },
            {
                "Hour18", hour16
            },
            {
                "Hour19", hour16
            },
            {
                "Hour20", hour16
            },
            {
                "Hour21", hour16
            },
            {
                "Hour22", hour22
            },
            {
                "Hour23", hour22
            },
        };
    }

    public Group GetGroup(DateTime date)
    {
        return new Group
        {
            GroupID = 1201,
            Label = "HUI Allen",
            SchedulerDays = new Dictionary<DateTime, Dictionary<string, bool?>>()
            {
                {
                    date, GetDefaultHoursDictionary(false, true, false, true, false)
                }
            },
            ServiceFuntions =
            [
                new ServiceFuntion()
                {
                    Label = "KNRM schipper",
                    OccupancyDays = new Dictionary<DateTime, Dictionary<string, bool?>>()
                    {
                        {
                            date, GetDefaultHoursDictionary(false, false, true, false, false)
                        }
                    }
                },
                new ServiceFuntion()
                {
                    Label = "KNRM opstapper",
                    OccupancyDays = new Dictionary<DateTime, Dictionary<string, bool?>>()
                    {
                        {
                            date, GetDefaultHoursDictionary(false, true, true, false, false)
                        }
                    }
                },
                new ServiceFuntion()
                {
                    Label = "KNRM Aank. Opstapper",
                    OccupancyDays = new Dictionary<DateTime, Dictionary<string, bool?>>()
                    {
                        {
                            date, GetDefaultHoursDictionary(true, false, true, false, true)
                        }
                    }
                },
            ]
        };
    }
}