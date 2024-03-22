using System.ComponentModel.DataAnnotations;
using MedicalInformationSystem.Models;

namespace MedicalInformationSystem.Data.Entities;

public class ConsultationEntity
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    
    
    public InspectionEntity Inspection { get; set; }
    public SpecialityEntity Speciality { get; set; }
    public IEnumerable<CommentEntity> Comment { get; set; }
}