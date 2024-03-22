using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class PatientModel : PatientCreateModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
}