using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.SharedForTests.Helpers;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

public static class SeedTargets
{
    internal static Guid TrainingTargetSubjectAlgemeneKennis;
    private static Guid _trainingTargetSubjectAlgemeneCommunicatie;
    private static Guid _trainingTargetSubjectTouwhandelingen;
    private static Guid _trainingTargetSubjectWalEnWater;
    private static Guid _trainingTargetSubjectCommunicatieOpHetWater;

    public static void Seed(DataContext dataContext, Guid customerId)
    {
        TrainingTargetSubjectAlgemeneKennis = Guid.NewGuid();
        _trainingTargetSubjectAlgemeneCommunicatie = Guid.NewGuid();
        _trainingTargetSubjectTouwhandelingen = Guid.NewGuid();
        _trainingTargetSubjectWalEnWater = Guid.NewGuid();
        _trainingTargetSubjectCommunicatieOpHetWater = Guid.NewGuid();
        SeedTrainingTargetSubjects(dataContext, customerId);
        SeedTrainingTargets(dataContext, customerId);
        dataContext.SaveChanges();
    }

    private static Guid SeedUser(DataContext dataContext)
    {
        var newId = Guid.NewGuid();
        dataContext.UsersGlobal.Add(new DbUsersGlobal()
        {
            Id = newId,
            Name = "xunit global user",
            CreatedOn = new DateTime(1992, 9, 4, 1, 4, 8, DateTimeKind.Utc),
            CreatedBy = Guid.NewGuid()
        });
        return newId;
    }

    private static void SeedTrainingTargetSubjects(DataContext dataContext, Guid customerId)
    {
        dataContext.TrainingTargetSubjects.Add(new DbTrainingTargetSubjects
        {
            Id = TrainingTargetSubjectAlgemeneKennis,
            CustomerId = customerId,
            Order = 10,
            Name = "Algemene kennis",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
        dataContext.TrainingTargetSubjects.Add(new DbTrainingTargetSubjects
        {
            Id = _trainingTargetSubjectTouwhandelingen,
            CustomerId = customerId,
            ParentId = TrainingTargetSubjectAlgemeneKennis,
            Order = 10,
            Name = "Touwhandelingen",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
        dataContext.TrainingTargetSubjects.Add(new DbTrainingTargetSubjects
        {
            Id = _trainingTargetSubjectAlgemeneCommunicatie,
            CustomerId = customerId,
            Order = 20,
            Name = "Communicatie",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
        dataContext.TrainingTargetSubjects.Add(new DbTrainingTargetSubjects
        {
            Id = _trainingTargetSubjectWalEnWater,
            CustomerId = customerId,
            ParentId = _trainingTargetSubjectAlgemeneCommunicatie,
            Order = 30,
            Name = "Wal en water",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
        dataContext.TrainingTargetSubjects.Add(new DbTrainingTargetSubjects
        {
            Id = _trainingTargetSubjectCommunicatieOpHetWater,
            CustomerId = customerId,
            ParentId = _trainingTargetSubjectAlgemeneCommunicatie,
            Order = 40,
            Name = "Communicatie op het water",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
    }

    private static void SeedTrainingTargets(DataContext dataContext, Guid customerId)
    {
        dataContext.TrainingTargets.Add(new DbTrainingTargets
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            SubjectId = _trainingTargetSubjectTouwhandelingen,
            Order = 10,
            Name = "De paalsteek",
            Type = TrainingTargetType.Knowledge,
            Group = TrainingTargetGroup.Single,
            Url = "https://kompas.knrm.nl/Algemene-kennis/Touwhandelingen/Touwhandelingen-paalsteek",
            UrlDescription = "Kompas",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
        dataContext.TrainingTargets.Add(new DbTrainingTargets
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            SubjectId = _trainingTargetSubjectTouwhandelingen,
            Order = 20,
            Name = "Een paalsteek leggen",
            Type = TrainingTargetType.Exercise,
            Group = TrainingTargetGroup.Single,
            Url = "https://kompas.knrm.nl/Algemene-kennis/Touwhandelingen/Touwhandelingen-paalsteek-leggen",
            UrlDescription = "Kompas",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
        dataContext.TrainingTargets.Add(new DbTrainingTargets
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            SubjectId = _trainingTargetSubjectWalEnWater,
            Order = 30,
            Name = "In- en uitmelden",
            Type = TrainingTargetType.Knowledge,
            Group = TrainingTargetGroup.Single,
            Url = "https://kompas.knrm.nl/Communicatie/Wal-en-water/In-en-uitmelden",
            UrlDescription = "Kompas",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
        dataContext.TrainingTargets.Add(new DbTrainingTargets
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            SubjectId = _trainingTargetSubjectWalEnWater,
            Order = 40,
            Name = "Uitvragen van de situatie",
            Type = TrainingTargetType.Knowledge,
            Group = TrainingTargetGroup.Single,
            Url = "https://kompas.knrm.nl/Communicatie/Wal-en-water/Uitvragen-van-de-situatie",
            UrlDescription = "Kompas",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
        dataContext.TrainingTargets.Add(new DbTrainingTargets
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            SubjectId = _trainingTargetSubjectWalEnWater,
            Order = 50,
            Name = "Uitvragen van de situatie",
            Type = TrainingTargetType.Exercise,
            Group = TrainingTargetGroup.Group,
            Url = "https://kompas.knrm.nl/Communicatie/Wal-en-water/Communicatie-uitvragen-van-de-situatie",
            UrlDescription = "Kompas",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
        dataContext.TrainingTargets.Add(new DbTrainingTargets
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            SubjectId = _trainingTargetSubjectCommunicatieOpHetWater,
            Order = 50,
            Name = "Werken met DSC",
            Type = TrainingTargetType.Knowledge,
            Group = TrainingTargetGroup.Single,
            Url = "https://kompas.knrm.nl/Communicatie/Communicatie-op-het-water/Werken-met-DSC",
            UrlDescription = "Kompas",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
        dataContext.TrainingTargets.Add(new DbTrainingTargets
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            SubjectId = _trainingTargetSubjectCommunicatieOpHetWater,
            Order = 60,
            Name = "SITREP",
            Type = TrainingTargetType.Knowledge,
            Group = TrainingTargetGroup.Single,
            Url = "https://kompas.knrm.nl/Communicatie/Communicatie-op-het-water/SITREP",
            UrlDescription = "Kompas",
            CreatedOn = new DateTime(2025, 08, 14, 12, 12, 12, DateTimeKind.Utc),
            CreatedBy = DefaultSettingsHelperMock.IdDefaultUserForTests
        });
    }
}