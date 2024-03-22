using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class PatientCreateModel
{
    [Required]
    [MaxLength(1000), MinLength(1)]
    public string Name { get; set; }
    public DateTime BirthDateTime { get; set; }
    [Required]
    public Gender Gender { get; set; }
}