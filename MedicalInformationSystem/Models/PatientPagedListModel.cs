namespace MedicalInformationSystem.Models;

public class PatientPagedListModel
{
    public IEnumerable<PatientModel>? Patients { get; set; }
    public PageInfoModel Pagination { get; set; }
}