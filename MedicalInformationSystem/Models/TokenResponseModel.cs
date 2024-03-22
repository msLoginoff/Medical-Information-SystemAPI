using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class TokenResponseModel
{
    [Required] 
    [MinLength(1)]
    public string Token { get; set; }
}