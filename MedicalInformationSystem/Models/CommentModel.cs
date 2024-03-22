using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class CommentModel : CommentCreateModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    public DateTime? ModifiedDate { get; set; }
    [Required]
    public Guid AuthorId { get; set; }
    [Required]
    [MinLength(1)]
    public string Author { get; set; }
}