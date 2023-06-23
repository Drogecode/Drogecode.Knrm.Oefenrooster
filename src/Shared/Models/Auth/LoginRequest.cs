using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Auth;

public class LoginRequest
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}
