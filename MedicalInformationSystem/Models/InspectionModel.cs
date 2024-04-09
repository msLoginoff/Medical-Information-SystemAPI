using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class InspectionModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    public DateTime Date { get; set; }
    [MaxLength(5000)]
    public string? Anamnesis { get; set; }
    [MaxLength(5000), MinLength(1)]
    public string? Complaints { get; set; }
    [MaxLength(5000), MinLength(1)]
    public string? Treatment { get; set; }
    [Required]
    public ConclusionEnum Conclusion { get; set; }
    public DateTime? NextVisitDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public Guid? BaseInspectionId { get; set; }
    public Guid? PreviousInspectionId { get; set; }
    public PatientModel Patient { get; set; }
    public DoctorModel Doctor { get; set; }
    [Required]
    [MinLength(1)]
    public IEnumerable<DiagnosisModel> Diagnoses { get; set; }
    public IEnumerable<InspectionConsultationModel>? Consultations { get; set; }
}