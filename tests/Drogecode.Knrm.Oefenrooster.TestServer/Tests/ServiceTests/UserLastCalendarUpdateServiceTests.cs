﻿using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ServiceTests;

public class UserLastCalendarUpdateServiceTests : BaseTest
{
    private readonly IUserLastCalendarUpdateService _userLastCalendarUpdateService;

    public UserLastCalendarUpdateServiceTests(
        IUserLastCalendarUpdateService userLastCalendarUpdateService,
        TestService testService) :
        base(testService)
    {
        _userLastCalendarUpdateService = userLastCalendarUpdateService;
    }

    [Fact]
    public async Task AddOrUpdateLastUpdateUserTest()
    {
        var result = await _userLastCalendarUpdateService.AddOrUpdateLastUpdateUser(Tester.DefaultCustomerId, Tester.DefaultUserId, CancellationToken.None);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetLastUpdateUsersJustSetTest()
    {
        try
        {
            var dummyDateTime = new DateTime(2020, 9, 4, 12, 8, 1);
            Tester.DateTimeServiceMock.SetMockDateTime(dummyDateTime);
            await _userLastCalendarUpdateService.AddOrUpdateLastUpdateUser(Tester.DefaultCustomerId, Tester.DefaultUserId, CancellationToken.None);
            Tester.DateTimeServiceMock.SetMockDateTime(dummyDateTime.AddMinutes(1));
            var result = await _userLastCalendarUpdateService.GetLastUpdateUsers(CancellationToken.None);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            Tester.DateTimeServiceMock.SetMockDateTime(null);
        }
    }

    [Fact]
    public async Task GetLastUpdateUsersInPeriodTest()
    {
        try
        {
            var dummyDateTime = new DateTime(2020, 9, 4, 12, 8, 1);
            Tester.DateTimeServiceMock.SetMockDateTime(dummyDateTime);
            await _userLastCalendarUpdateService.AddOrUpdateLastUpdateUser(Tester.DefaultCustomerId, Tester.DefaultUserId, CancellationToken.None);
            Tester.DateTimeServiceMock.SetMockDateTime(dummyDateTime.AddMinutes(30));
            var result = await _userLastCalendarUpdateService.GetLastUpdateUsers(CancellationToken.None);
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }
        finally
        {
            Tester.DateTimeServiceMock.SetMockDateTime(null);
        }
    }

    [Fact]
    public async Task GetLastUpdateUsersAfterPeriodTest()
    {
        try
        {
            var dummyDateTime = new DateTime(2020, 9, 4, 12, 8, 1, DateTimeKind.Utc);
            Tester.DateTimeServiceMock.SetMockDateTime(dummyDateTime);
            await _userLastCalendarUpdateService.AddOrUpdateLastUpdateUser(Tester.DefaultCustomerId, Tester.DefaultUserId, CancellationToken.None);
            Tester.DateTimeServiceMock.SetMockDateTime(dummyDateTime.AddHours(2));
            var result = await _userLastCalendarUpdateService.GetLastUpdateUsers(CancellationToken.None);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            Tester.DateTimeServiceMock.SetMockDateTime(null);
        }
    }
}