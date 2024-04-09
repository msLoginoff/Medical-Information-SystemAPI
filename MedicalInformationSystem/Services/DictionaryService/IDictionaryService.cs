using MedicalInformationSystem.Models;

namespace MedicalInformationSystem.Services.DictionaryService;

public interface IDictionaryService
{
    public SpecialtiesPagedListModel GetSpecialities(string? name,
        int page = 1,
        int size = 5);
}