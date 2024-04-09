using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MedicalInformationSystem.Models;

public class DoctorEditModel
{
    [Required]
    [MinLength(1), EmailAddress]
    public string Email { get; set; }
    [Required]
    [MaxLength(1000), MinLength(1)]
    public string Name { get; set; }
    public DateTime BirthDateTime { get; set; }
    [Required]
    public Gender Gender { get; set; }
    [Phone]
    public string? Phone { get; set; }
}