using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class LoginCredentialsModel
{
    [Required]
    [MinLength(1), EmailAddress]
    public EmailAddressAttribute Email { get; set; }
    [Required]
    [MinLength(1), PasswordPropertyText] //todo return to password 
    public PasswordPropertyTextAttribute Password { get; set; }
}