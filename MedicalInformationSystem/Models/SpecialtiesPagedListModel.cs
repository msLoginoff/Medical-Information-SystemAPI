namespace MedicalInformationSystem.Models;

public class SpecialtiesPagedListModel
{
    public IEnumerable<SpecialityModel>? Specialties { get; set; }
    public PageInfoModel Pagination { get; set; }
}