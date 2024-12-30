using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Menu;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class MenuMapper
{
    public static DrogeMenu ToDrogeMenu(this DbMenu dbMenu)
    {
        return new DrogeMenu
        {
            Id = dbMenu.Id,
            ParentId = dbMenu.ParentId,
            AddLoginHint = dbMenu.AddLoginHint,
            IsGroup = dbMenu.IsGroup,
            TargetBlank = dbMenu.TargetBlank,
            Order = dbMenu.Order,
            Text = dbMenu.Text,
            Url = dbMenu.Url
        };
    }

    public static DbMenu ToDb(this DrogeMenu drogeMenu, Guid customerId)
    {
        return new DbMenu
        {
            Id = drogeMenu.Id,
            CustomerId = customerId,
            ParentId = drogeMenu.ParentId,
            AddLoginHint = drogeMenu.AddLoginHint,
            IsGroup = drogeMenu.IsGroup,
            TargetBlank = drogeMenu.TargetBlank,
            Order = drogeMenu.Order,
            Text = drogeMenu.Text,
            Url = drogeMenu.Url
        };
    }
}