using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class InspectionCreateModel
{
    [Required]
    public DateTime Date { get; set; }
    [Required]
    [MaxLength(5000)]
    public string Anamnesis { get; set; }
    [Required]
    [MaxLength(5000), MinLength(1)]
    public string Complaints { get; set; }
    [Required]
    [MaxLength(5000), MinLength(1)]
    public string Treatment { get; set; }
    [Required]
    public ConclusionEnum Conclusion { get; set; }
    public DateTime? NextVisitDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public Guid? PreviousInspectionId { get; set; }
    [Required]
    [MinLength(1)]
    public DiagnosisCreateModel Diagnoses { get; set; }
    public ConsultationCreateModel? Consultations { get; set; }
}