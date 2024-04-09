using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class LoginCredentialsModel
{
    [Required]
    [MinLength(1), EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(1), PasswordPropertyText]
    public string Password { get; set; }
}