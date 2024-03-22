using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Data.Entities;

public class SpecialityEntity
{
    [Required] 
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    [Required]
    public string Name { get; set; }
    
    public IEnumerable<DoctorEntity>? Doctor { get; set; }
    public IEnumerable<ConsultationEntity>? Consultation { get; set; }
}