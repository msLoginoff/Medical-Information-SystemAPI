namespace MedicalInformationSystem.Models;

public class Icd10SearchModel
{
    public IEnumerable<Icd10RecordModel>? Records { get; set; }
    public PageInfoModel Pagination { get; set; }
}