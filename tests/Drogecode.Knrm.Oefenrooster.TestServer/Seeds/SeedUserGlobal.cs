using Drogecode.Knrm.Oefenrooster.Server.Database.Models;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

public static class SeedUserGlobal
{
    public static Guid Seed(DataContext dataContext)
    {
        var newId = SeedUser(dataContext);
        dataContext.SaveChanges();
        return newId;
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
}