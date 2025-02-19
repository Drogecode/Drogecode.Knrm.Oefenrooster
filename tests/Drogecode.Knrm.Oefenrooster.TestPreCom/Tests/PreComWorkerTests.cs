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
        result.Should().Be("Voor aankomend uur hebben we nog een schipper nodig opstapper nodig algemeen nodig ");
    }

    private IPreComClient MockClient()
    {
        var mockClient = Substitute.For<IPreComClient>();
        mockClient.GetAllUserGroups().Returns([new Group { GroupID = 1201 }]);
        
        mockClient.GetAllFunctions(1201, DateTime.Today).Returns(new Group
        {
            GroupID = 1201,
            Label = "HUI Allen",
            SchedulerDays = new Dictionary<DateTime, Dictionary<string, bool?>>()
            {
                {
                    DateTime.Today, DefaultHoursDictionary
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
                            DateTime.Today, DefaultHoursDictionary
                        }
                    }
                },
                new ServiceFuntion()
                {
                    Label = "KNRM opstapper",
                    OccupancyDays = new Dictionary<DateTime, Dictionary<string, bool?>>()
                    {
                        {
                            DateTime.Today, DefaultHoursDictionary
                        }
                    }
                },
                new ServiceFuntion()
                {
                    Label = "KNRM Aank. Opstapper",
                    OccupancyDays = new Dictionary<DateTime, Dictionary<string, bool?>>()
                    {
                        {
                            DateTime.Today, DefaultHoursDictionary
                        }
                    }
                },
            ]
        });
        mockClient.GetOccupancyLevels(1201, DateTime.Today, DateTime.Today.AddDays(7)).Returns(new Dictionary<DateTime, int>()
        {
            {
                DateTime.Today, 1
            }
        });
        return mockClient;
    }
    
    Dictionary<string, bool?> DefaultHoursDictionary = new Dictionary<string, bool?>()
    {
        {
            "Hour0", true
        },
        {
            "Hour1", true
        },
        {
            "Hour2", true
        },
        {
            "Hour3", true
        },
        {
            "Hour4", true
        },
        {
            "Hour5", true
        },
        {
            "Hour6", true
        },
        {
            "Hour7", true
        },
        {
            "Hour8", true
        },
        {
            "Hour9", true
        },
        {
            "Hour10", true
        },
        {
            "Hour11", true
        },
        {
            "Hour12", true
        },
        {
            "Hour13", true
        },
        {
            "Hour14", true
        },
        {
            "Hour15", true
        },
        {
            "Hour16", true
        },
        {
            "Hour17", true
        },
        {
            "Hour18", true
        },
        {
            "Hour19", true
        },
        {
            "Hour20", true
        },
        {
            "Hour21", true
        },
        {
            "Hour22", true
        },
        {
            "Hour23", true
        },
    };
}