using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class DiagnosisBase
{
    [MaxLength(5000)]
    public string? Description { get; set; }
    [Required]
    public DiagnosisTypeEnum Type { get; set; }
}