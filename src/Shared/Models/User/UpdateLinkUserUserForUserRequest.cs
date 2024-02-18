using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.User;

public class UpdateLinkUserUserForUserRequest
{
    public Guid UserAId { get; set; }
    public Guid UserBId { get; set; }
    public UserUserLinkType LinkType { get; set; }
    public bool Add {  get; set; } = true;
}
