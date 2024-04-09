using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalInformationSystem.Data.Entities;

public class PasswordEntity
{
    public Guid Id { get; set; }
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
    public long TokenSeries { get; set; }
    public DoctorEntity Doctor { get; set; }
}