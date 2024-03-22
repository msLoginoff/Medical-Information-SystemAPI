using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class DiagnosisCreateModel : DiagnosisBase
{
    [Required]
    public Guid IcdDiagnosisId { get; set; }
}