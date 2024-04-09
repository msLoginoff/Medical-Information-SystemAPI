using MedicalInformationSystem.Models;

namespace MedicalInformationSystem.Services.DictionaryService;

public interface IDictionaryService
{
    public SpecialtiesPagedListModel GetSpecialities(string? name,
        int page = 1,
        int size = 5);

    public Icd10SearchModel GetIcd(string? request,
        int page,
        int size);

    public IEnumerable<Icd10RecordModel> GetIcdRoots();
}