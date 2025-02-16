using Drogecode.Knrm.Oefenrooster.Server.Database.Models;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

public class SeedMenu
{
    public static void Seed(DataContext dataContext, Guid defaultCustomerId)
    {
        SeedMenus(dataContext, defaultCustomerId);
        dataContext.SaveChanges();
    }
    
    private static void SeedMenus(DataContext dataContext, Guid defaultCustomerId)
    {
        var parent = Guid.CreateVersion7();
        dataContext.Menus.Add(new DbMenu
        {
            Id = parent,
            ParentId = null,
            CustomerId = defaultCustomerId,
            Text = "Handige links",
            Url = "",
            IsGroup = true,
            AddLoginHint = null,
            TargetBlank = false,
            Order = 10,
        });
        dataContext.Menus.Add(new DbMenu
        {
            Id = Guid.CreateVersion7(),
            ParentId = parent,
            CustomerId = defaultCustomerId,
            Text = "Sharepoint",
            Url = "https://dorus1824.sharepoint.com",
            IsGroup = false,
            AddLoginHint = '?',
            TargetBlank = true,
            Order = 20,
        });
        dataContext.Menus.Add(new DbMenu
        {
            Id = Guid.CreateVersion7(),
            ParentId = parent,
            CustomerId = defaultCustomerId,
            Text = "LPLH",
            Url = "https://dorus1824.sharepoint.com/:b:/r/sites/KNRM/Documenten/EHBO/LPLH/20181115%20LPLH_KNRM_1_1.pdf?csf=1&web=1&e=4L3VPo",
            IsGroup = false,
            AddLoginHint = '&',
            TargetBlank = true,
            Order = 30,
        });
    }
}