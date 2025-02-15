﻿using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.User;

public class UpdateLinkUserUserForUserRequest
{
    public Guid UserAId { get; set; }
    public Guid UserBId { get; set; }
    public UserUserLinkType LinkType { get; set; }
    public bool Add {  get; set; } = true;
}
