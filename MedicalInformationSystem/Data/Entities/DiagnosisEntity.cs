using System.ComponentModel.DataAnnotations;
using MedicalInformationSystem.Models;

namespace MedicalInformationSystem.Data.Entities;

public class DiagnosisEntity
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    [Required]
    public string Code { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    [Required]
    public DiagnosisTypeEnum Type { get; set; }
    /*[Required]
    public Guid IcdDiagnosisId { get; set; }*/
    
    public InspectionEntity Inspection { get; set; }
}