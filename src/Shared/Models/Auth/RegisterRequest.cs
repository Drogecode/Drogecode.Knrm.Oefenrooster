using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Auth;

public class RegisterRequest
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match!")]
    public string PasswordConfirm { get; set; }
}
