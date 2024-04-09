using System.ComponentModel.DataAnnotations;
using MedicalInformationSystem.Data.Entities;

namespace MedicalInformationSystem.Models;

public class InspectionShortModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public DiagnosisModel Diagnosis { get; set; }
}