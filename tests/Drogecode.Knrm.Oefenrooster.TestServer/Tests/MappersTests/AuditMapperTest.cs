using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.MappersTests;

public class AuditMapperTest
{
    
    [Fact]
    public void MapperTest()
    {
        var userInNote = Guid.CreateVersion7();
        var dbAudit = new DbAudit
        {
            Id = Guid.CreateVersion7(),
            UserId = Guid.CreateVersion7(),
            CustomerId = Guid.CreateVersion7(),
            AuditType = AuditType.AddUser,
            Note = JsonSerializer.Serialize(new AuditAssignedUser
            {
                UserId = userInNote, Assigned = true, Availability = Availability.Available, SetBy = AvailabilitySetBy.User,
                AuditReason = AuditReason.Assigned
            }),
            ObjectKey = Guid.CreateVersion7(),
            ObjectName = "objectName",
            Created = DateTime.Now,
        };
        var trainingAudit = dbAudit.ToTrainingAudit();
        trainingAudit.Should().NotBeNull();
        trainingAudit.UserId.Should().Be(userInNote);
        trainingAudit.TrainingId.Should().Be(dbAudit.ObjectKey);
        trainingAudit.AuditType.Should().Be(dbAudit.AuditType);
        trainingAudit.Assigned.Should().BeTrue();
        trainingAudit.Date.Should().Be(dbAudit.Created);
        trainingAudit.Availability.Should().Be(Availability.Available);
        trainingAudit.SetBy.Should().Be(AvailabilitySetBy.User);
        trainingAudit.AuditReason.Should().Be(AuditReason.Assigned);
    }
}