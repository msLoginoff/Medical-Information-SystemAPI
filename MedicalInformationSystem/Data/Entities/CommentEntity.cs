using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Data.Entities;

public class CommentEntity
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    public DateTime? ModifiedDate { get; set; }
    [Required]
    public string Content { get; set; }
    
    public ConsultationEntity Consultation { get; set; }
    public DoctorEntity Author { get; set; }
    public CommentEntity? Parent { get; set; }
    public IEnumerable<CommentEntity>? NestedComment { get; set; }
}