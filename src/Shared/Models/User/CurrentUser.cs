﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.User;

public class CurrentUser
{
    public Guid? Id { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? UserName { get; set; }
    public List<KeyValuePair<string, string>> Claims { get; set; }
}
