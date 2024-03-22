using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class DoctorEditModel
{
    [Required]
    [MinLength(1), EmailAddress]
    public EmailAddressAttribute Email { get; set; }
    [Required]
    [MaxLength(1000), MinLength(1)]
    public string Name { get; set; }
    public DateTime BirthDateTime { get; set; }
    [Required]
    public Gender Gender { get; set; }
    [Phone]
    public PhoneAttribute? Phone { get; set; }
}