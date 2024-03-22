using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Data.Entities;

public class PasswordEntity
{
    [Key]
    public DoctorEntity Doctor { get; set; }
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
    public long TokenSeries { get; set; }
}