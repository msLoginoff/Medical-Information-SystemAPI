using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class DoctorRegisterModel : DoctorEditModel
{
    [Required]
    [MinLength(1), PasswordPropertyText]
    public string Password { get; set; }
    [Required]
    public Guid Speciality { get; set; }
}