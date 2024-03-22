using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class Icd10RecordModel
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
}