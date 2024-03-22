using System.ComponentModel.DataAnnotations;
using MedicalInformationSystem.Models;

namespace MedicalInformationSystem.Data.Entities;

public class PatientEntity
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    [Required]
    public string Name { get; set; }
    public DateTime BirthDateTime { get; set; }
    [Required]
    public Gender Gender { get; set; }
    
    public IEnumerable<InspectionEntity>? Inspection { get; set; }
}