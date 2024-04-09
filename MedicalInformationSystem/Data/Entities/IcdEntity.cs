using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Data.Entities;

public class IcdEntity
{
    [Key]
    public Guid NewId { get; set; }
    public int ID { get; set; }
    public string MKB_CODE { get; set; }
    public string MKB_NAME { get; set; }
    public string ID_PARENT { get; set; }
    public IcdRootsEntity IcdRoot { get; set; }
}