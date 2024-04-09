using System.Text.RegularExpressions;
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

    public Icd10SearchModel GetIcd(string? request, int page, int size)
    {
        var icd = _context.Icd.AsQueryable();
        if (request != null)
        {
            var regex = new Regex("[A-Z][0-9][0-9]");
            if (regex.IsMatch(request))
            {
                icd = icd.Where(x => x.MKB_CODE.Contains(request));
            }
            else
            {
                icd = icd.Where(x => x.MKB_NAME.Contains(request));
            }
            
        }

        var date = new System.DateTime(2024, 3, 20, 16, 22, 19);
        var pagination = new PageInfoModel(size, icd.Count(), page);

        var icdResponse = new Icd10SearchModel
        {
            Records = icd.OrderBy(x => x.MKB_CODE).Skip((page - 1) * size)
                .Take(size).Select(i => new Icd10RecordModel
            {
                Id = i.NewId,
                CreateTime = date.ToUniversalTime(),
                Code = i.MKB_CODE,
                Name = i.MKB_NAME
            }),
            Pagination = pagination
        };

        return icdResponse;
    }

    public IEnumerable<Icd10RecordModel> GetIcdRoots()
    {
        var roots = _context.IcdRoots.OrderBy(x => x.ID).AsQueryable();
        var date = new System.DateTime(2024, 3, 20, 16, 22, 19);
        
        return roots.Select(x => new Icd10RecordModel
        {
            Id = x.NewId,
            CreateTime = date.ToUniversalTime(),
            Code = x.MKB_CODE,
            Name = x.MKB_NAME
        }).AsEnumerable();
    }
}