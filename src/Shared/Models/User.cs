﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
    }
}
