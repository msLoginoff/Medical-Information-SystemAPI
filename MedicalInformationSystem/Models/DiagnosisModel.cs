using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class DiagnosisModel : DiagnosisBase
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    [Required]
    [MinLength(1)]
    public string Code { get; set; }
    [Required]
    [MinLength(1)]
    public string Name { get; set; }
}