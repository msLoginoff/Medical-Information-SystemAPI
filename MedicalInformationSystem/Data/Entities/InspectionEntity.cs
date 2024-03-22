using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MedicalInformationSystem.Models;

namespace MedicalInformationSystem.Data.Entities;

public class InspectionEntity
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public string Anamnesis { get; set; }
    [Required]
    public string Complaints { get; set; }
    [Required]
    public string Treatment { get; set; }
    [Required]
    public ConclusionEnum Conclusion { get; set; }
    public DateTime? NextVisitDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public bool HasChain { get; set; }
    public bool HasNested { get; set; }
    
    public PatientEntity Patient { get; set; }
    [ForeignKey("PreviousInspectionId")]
    public InspectionEntity? PreviousInspection { get; set; }
    [ForeignKey("NextInspectionId")]
    public InspectionEntity? NextInspection { get; set; }
    [Required]
    public IEnumerable<DiagnosisEntity> Diagnoses { get; set; }
    public IEnumerable<ConsultationEntity>? Consultations { get; set; }
    public DoctorEntity Doctor { get; set; }
}