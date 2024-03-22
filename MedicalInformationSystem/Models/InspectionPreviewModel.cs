using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class InspectionPreviewModel : InspectionShortModel
{
    public Guid? PreviousId { get; set; }
    [Required]
    public ConclusionEnum Conclusion { get; set; }
    [Required]
    public Guid DoctorId { get; set; }
    [Required]
    [MinLength(1)]
    public string Doctor { get; set; }
    [Required]
    public Guid PatientId { get; set; }
    [Required]
    public string Patient { get; set; }
    public bool HasChain { get; set; }
    public bool HasNested { get; set; }
}