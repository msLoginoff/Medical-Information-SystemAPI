using MedicalInformationSystem.Data;
using MedicalInformationSystem.Models;

namespace MedicalInformationSystem.Services.DictionaryService;

public class DictionaryService : IDictionaryService
{
    private readonly AppDbContext _context;

    public DictionaryService(AppDbContext context)
    {
        _context = context;
    }
    
    public SpecialtiesPagedListModel GetSpecialities(string? name, int page, int size)
    {
        var foundSpecialities = _context.Speciality.Where(x => (name != null && x.Name.Contains(name)) || name == null);
        var pagination = new PageInfoModel(size, foundSpecialities.Count(), page);
        return new SpecialtiesPagedListModel
        {
            Specialties = foundSpecialities.Select(x => new SpecialityModel
            {
                Name = x.Name,
                Id = x.Id,
                CreateTime = x.CreateTime
            }),
            Pagination = pagination
        };
    }
}