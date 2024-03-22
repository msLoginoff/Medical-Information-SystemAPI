using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MedicalInformationSystem.Models;

namespace MedicalInformationSystem.Data.Entities;

public class DoctorEntity
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Name { get; set; }
    public DateTime BirthDateTime { get; set; }
    [Required]
    public Gender Gender { get; set; }
    [Phone]
    public string? Phone { get; set; }
    [Required]
    
    public IEnumerable<InspectionEntity> Inspection { get; set; }
    public IEnumerable<CommentEntity> Comment { get; set; }
    public SpecialityEntity Speciality { get; set; }
}