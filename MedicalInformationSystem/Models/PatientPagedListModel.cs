namespace MedicalInformationSystem.Models;

public class PatientPagedListModel
{
    public IEnumerator<PatientModel>? Patients { get; set; }
    public PageInfoModel Pagination { get; set; }
}