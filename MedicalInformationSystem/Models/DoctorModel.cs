using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class DoctorModel : DoctorEditModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
}