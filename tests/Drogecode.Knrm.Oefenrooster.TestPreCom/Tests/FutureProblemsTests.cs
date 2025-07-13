using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.PreCom.Interfaces;
using Drogecode.Knrm.Oefenrooster.PreCom.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.SharedForTests.Providers;
using Drogecode.Knrm.Oefenrooster.SharedForTests.Providers.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Drogecode.Knrm.Oefenrooster.TestPreCom.Tests;

public class FutureProblemsTests : IDisposable
{
    private readonly ILogger _logger;
    private readonly IDateTimeProviderMock _dateTimeProviderMock;

    public FutureProblemsTests(ILoggerProvider loggerProvider)
    {
        _logger = loggerProvider.CreateLogger("PreComWorkerTests");
        _dateTimeProviderMock = new DateTimeProviderMock();
        _dateTimeProviderMock.SetMockDateTime(new DateTime(2022, 3, 18, 12, 2, 1));
    }

    [Fact]
    public async Task NextHourTest()
    {
        var mockClient = MockClient();
        var worker = new FutureProblems(mockClient, _logger, _dateTimeProviderMock);
        var result = await worker.Work(NextRunMode.NextHour);
        Assert.NotNull(result.Problems);
        result.Problems.Trim().Should().Be("Voor aankomend uur hebben we nog een schipper nodig opstapper nodig aankomend opstapper nodig");
    }

    [Fact]
    public async Task TodayTomorrowTest()
    {
        var mockClient = MockClient();
        var worker = new FutureProblems(mockClient, _logger, _dateTimeProviderMock);
        var result = await worker.Work(NextRunMode.TodayTomorrow);
        Assert.NotNull(result.Problems);
        result.Problems.Trim().Should().Be("{0}<br />Schipper<br />van 12 tot 16<br />Opstapper<br />van 12 tot 16<br />Aankomend opstapper<br />van 12 tot 16<br />van 22 tot 24<br /><br />{1}<br />Schipper<br />van 8 tot 16<br />Opstapper<br />van 2 tot 16<br />Aankomend opstapper<br />van 0 tot 2<br />van 8 tot 16<br />van 22 tot 24");
    }

    [Fact]
    public async Task NextWeekTest()
    {
        var mockClient = MockClient();
        var worker = new FutureProblems(mockClient, _logger, _dateTimeProviderMock);
        var result = await worker.Work(NextRunMode.NextWeek);
        Assert.NotNull(result.Problems);
        result.Problems.Trim().Should().Be("{0}<br />Schipper<br />van 12 tot 16<br />Opstapper<br />van 12 tot 16<br />Aankomend opstapper<br />van 12 tot 16<br />van 22 tot 24<br /><br />{1}<br />Schipper<br />van 8 tot 16<br />Opstapper<br />van 2 tot 16<br />Aankomend opstapper<br />van 0 tot 2<br />van 8 tot 16<br />van 22 tot 24<br /><br />{2}<br />Schipper<br />van 8 tot 16<br />Opstapper<br />van 2 tot 16<br />Aankomend opstapper<br />van 0 tot 2<br />van 8 tot 16<br />van 22 tot 24<br /><br />{3}<br />Schipper<br />van 8 tot 16<br />Opstapper<br />van 2 tot 16<br />Aankomend opstapper<br />van 0 tot 2<br />van 8 tot 16<br />van 22 tot 24<br /><br />{4}<br />Schipper<br />van 8 tot 16<br />Opstapper<br />van 2 tot 16<br />Aankomend opstapper<br />van 0 tot 2<br />van 8 tot 16<br />van 22 tot 24");
    }

    private IPreComClient MockClient()
    {
        var mockClient = Substitute.For<IPreComClient>();
        mockClient.GetAllUserGroups().Returns([new Group { GroupID = 1201 }]);

        mockClient.GetAllFunctions(1201, _dateTimeProviderMock.Today()).Returns(GetGroup(_dateTimeProviderMock.Today()));
        mockClient.GetAllFunctions(1201, _dateTimeProviderMock.Today().AddDays(1)).Returns(GetGroup(_dateTimeProviderMock.Today().AddDays(1)));
        mockClient.GetAllFunctions(1201, _dateTimeProviderMock.Today().AddDays(2)).Returns(GetGroup(_dateTimeProviderMock.Today().AddDays(2)));
        mockClient.GetAllFunctions(1201, _dateTimeProviderMock.Today().AddDays(3)).Returns(GetGroup(_dateTimeProviderMock.Today().AddDays(3)));
        mockClient.GetAllFunctions(1201, _dateTimeProviderMock.Today().AddDays(4)).Returns(GetGroup(_dateTimeProviderMock.Today().AddDays(4)));
        mockClient.GetAllFunctions(1201, _dateTimeProviderMock.Today().AddDays(5)).Returns(GetGroup(_dateTimeProviderMock.Today().AddDays(5)));
        mockClient.GetAllFunctions(1201, _dateTimeProviderMock.Today().AddDays(6)).Returns(GetGroup(_dateTimeProviderMock.Today().AddDays(6)));
        mockClient.GetAllFunctions(1201, _dateTimeProviderMock.Today().AddDays(7)).Returns(GetGroup(_dateTimeProviderMock.Today().AddDays(7)));
        mockClient.GetOccupancyLevels(1201, _dateTimeProviderMock.Today(), _dateTimeProviderMock.Today().AddDays(8)).Returns(new Dictionary<DateTime, int>()
        {
            {
                _dateTimeProviderMock.Today(), -1
            },
            {
                _dateTimeProviderMock.Today().AddDays(1), -1
            },
            {
                _dateTimeProviderMock.Today().AddDays(2), -1
            },
            {
                _dateTimeProviderMock.Today().AddDays(3), 1
            },
            {
                _dateTimeProviderMock.Today().AddDays(4), -1
            },
            {
                _dateTimeProviderMock.Today().AddDays(5), 1
            },
            {
                _dateTimeProviderMock.Today().AddDays(6), -1
            },
            {
                _dateTimeProviderMock.Today().AddDays(7), 1
            }
        });
        mockClient.GetOccupancyLevels(1201, _dateTimeProviderMock.Today(), _dateTimeProviderMock.Today().AddDays(2)).Returns(new Dictionary<DateTime, int>()
        {
            {
                _dateTimeProviderMock.Today(), -1
            },
            {
                _dateTimeProviderMock.Today().AddDays(1), -1
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

    public void Dispose()
    {
        _dateTimeProviderMock.SetMockDateTime(null);
    }
}