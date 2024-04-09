using MedicalInformationSystem.Data;
using MedicalInformationSystem.Data.Entities;
using MedicalInformationSystem.Exceptions;
using MedicalInformationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalInformationSystem.Services.InspectionService;

public class InspectionService : IInspectionService
{
    private readonly AppDbContext _context;

    public InspectionService(AppDbContext context)
    {
        _context = context;
    }
    
    public InspectionModel GetInspectionInformation(Guid id)
    {
        var inspection = _context.Inspection
            .Include(inspectionEntity => inspectionEntity.PreviousInspection)
            .Include(inspectionEntity => inspectionEntity.Patient)
            .Include(inspectionEntity => inspectionEntity.Doctor)
            .Include(inspectionEntity => inspectionEntity.Diagnoses)
            .Include(inspectionEntity => inspectionEntity.Consultations)!
            .ThenInclude(consultationEntity => consultationEntity.Speciality)
            .Include(inspectionEntity => inspectionEntity.Consultations)!
            .ThenInclude(consultationEntity => consultationEntity.Comment)
            .ThenInclude(commentEntity => commentEntity.Parent)
            .Include(inspectionEntity => inspectionEntity.Consultations)!
            .ThenInclude(consultationEntity => consultationEntity.Comment)
            .ThenInclude(commentEntity => commentEntity.Author).FirstOrDefault(x => x.Id == id);

        if (inspection == null)
        {
            throw new NotFoundException($"Inspection with id={id} not found");
        }


        InspectionEntity? baseInspection = null;
        
        if (inspection.PreviousInspection != null)
        {
            var ptr = inspection.PreviousInspection;
            var currentInspection = inspection;
            
            while (ptr != null)
            {
                //currentInspection = ptr;
                currentInspection = _context.Inspection.Include(inspectionEntity => inspectionEntity.PreviousInspection)
                    .First(x => x == ptr);
                ptr = currentInspection?.PreviousInspection;
            }

            baseInspection = currentInspection;
        }
        
        var inspectionResponse = new InspectionModel
        {
            Date = inspection.Date,
            Anamnesis = inspection.Anamnesis,
            Complaints = inspection.Complaints,
            Treatment = inspection.Treatment,
            Conclusion = inspection.Conclusion,
            NextVisitDate = inspection.NextVisitDate,
            DeathDate = inspection.DeathDate,
            BaseInspectionId = baseInspection?.Id,
            PreviousInspectionId = inspection.PreviousInspection?.Id,
            Patient = new PatientModel
            {
                Name = inspection.Patient.Name,
                BirthDateTime = inspection.Patient.BirthDateTime,
                Gender = inspection.Patient.Gender,
                Id = inspection.Patient.Id,
                CreateTime = inspection.Patient.CreateTime
            },
            Doctor = new DoctorModel
            {
                Name = inspection.Doctor.Name,
                BirthDateTime = inspection.Doctor.BirthDateTime,
                Gender = inspection.Doctor.Gender,
                Email = inspection.Doctor.Email,
                Phone = inspection.Doctor.Phone,
                Id = inspection.Doctor.Id,
                CreateTime = inspection.CreateTime
            },
            Diagnoses = inspection.Diagnoses.Select(d => new DiagnosisModel
            {
                Code = d.Code,
                Name = d.Name,
                Description = d.Description,
                Type = d.Type,
                Id = d.Id,
                CreateTime = d.CreateTime
            }),
            Consultations = inspection.Consultations?.Select(c => new InspectionConsultationModel
            {
                Id = c.Id,
                CreateTime = c.CreateTime,
                InspectionId = inspection.Id,
                Speciality = new SpecialityModel
                {
                    Id = c.Speciality.Id,
                    CreateTime = c.Speciality.CreateTime,
                    Name = c.Speciality.Name
                },
                RootComment = c.Comment.Where(x => x.Parent == null && x.Consultation.Id == c.Id)
                    .Select(r => new InspectionCommentModel
                {
                    Id = r.Id,
                    CreateTime = r.CreateTime,
                    ParentId = r.Parent?.Id,
                    Content = r.Content,
                    Author = new DoctorModel
                    {
                        Id = r.Author.Id,
                        CreateTime = r.Author.CreateTime,
                        Name = r.Author.Name,
                        BirthDateTime = r.Author.BirthDateTime,
                        Gender = r.Author.Gender,
                        Email = r.Author.Email,
                        Phone = r.Author.Phone
                    },
                    ModifyTime = r.ModifiedDate
                }).First(),
                CommentsNumber = c.Comment.Count()
            }),
            
            Id = inspection.Id,
            CreateTime = inspection.CreateTime
        };

        return inspectionResponse;
    }

    public object? EditInspectionInformation(Guid id, Guid doctorId, InspectionEditModel inspectionEditModel)
    {
        var foundInspection = _context.Inspection
            .Include(inspectionEntity => inspectionEntity.PreviousInspection)
            .Include(inspectionEntity => inspectionEntity.Doctor)
            .FirstOrDefault(x => x.Id == id);

        if (foundInspection == null)
        {
            throw new NotFoundException($"Inspection with id={id} not found");
        }

        if (foundInspection.Doctor.Id != doctorId)
        {
            throw new Forbidden("This Doctor doesn't have editing rights (not the inspection author)");
        }

        var oldDiagnoses = _context.Diagnosis.Where(x => x.Inspection.Id == id);

        foreach (var oldDiagnosis in oldDiagnoses)
        {
            _context.Remove(oldDiagnosis);
        }
        
        foreach (var diagnosis in inspectionEditModel.Diagnoses)
        {
            var icd = _context.Icd.FirstOrDefault(x => x.NewId == diagnosis.IcdDiagnosisId);
            if (icd == null)
            {
                throw new NotFoundException("icdDiagnosisId not found");
            }

            var newDiagnosis = new DiagnosisEntity
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now.ToUniversalTime(),
                Code = icd.MKB_CODE,
                Description = diagnosis.Description,
                IcdDiagnosis = icd,
                Inspection = foundInspection,
                Name = icd.MKB_NAME,
                Type = diagnosis.Type
            };
            _context.Add(newDiagnosis);
        }

        if (inspectionEditModel.Conclusion == ConclusionEnum.Death && foundInspection.HasNested)
        {
            throw new BadRequest($"Inspection with conclusion Death can be only the latest in a chain");
        }
        if (inspectionEditModel.Diagnoses
                .Count(x => x.Type == DiagnosisTypeEnum.Main) != 1)
        {
            throw new BadRequest($"Inspection always must contain one diagnosis with Type equal to Main");
        }

        if (inspectionEditModel.Conclusion == ConclusionEnum.Disease)
        {
            if (inspectionEditModel.NextVisitDate == null)
            {
                throw new BadRequest($"Conclusion Disease requires not null correct date");
            }

            if (inspectionEditModel.NextVisitDate <= foundInspection.Date)
            {
                throw new BadRequest($"Next visit date can't be earlier this inspection date");
            }

            if (inspectionEditModel.DeathDate != null)
            {
                throw new BadRequest($"Conclusion Disease requires null Death date");
            }
        }
        

        if (inspectionEditModel.Conclusion == ConclusionEnum.Death)
        {
            if (inspectionEditModel.DeathDate == null)
            {
                throw new BadRequest($"Conclusion Death requires not null correct date and time");
            }

            if (inspectionEditModel.DeathDate > DateTime.Now)
            {
                throw new BadRequest($"Conclusion Death requires correct date and time, which can't be later than now {DateTime.Now}");
            }

            if (inspectionEditModel.DeathDate <= foundInspection.Date)
            {
                throw new BadRequest($"Death date can't be earlier this inspection date");
            }

            if (inspectionEditModel.NextVisitDate != null)
            {
                throw new BadRequest($"Conclusion Death requires null next visit date");
            }
        }

        if (inspectionEditModel.Conclusion == ConclusionEnum.Recovery &&
            (inspectionEditModel.NextVisitDate != null ||
             inspectionEditModel.DeathDate != null))
        {
            throw new BadRequest("Death date and Next Visit date must be empty in case of Recovery conclusion");
        }
        
        foundInspection.Anamnesis = inspectionEditModel.Anamnesis;
        foundInspection.Complaints = inspectionEditModel.Complaints;
        foundInspection.Treatment = inspectionEditModel.Treatment;
        foundInspection.Conclusion = inspectionEditModel.Conclusion;
        foundInspection.NextVisitDate = inspectionEditModel.NextVisitDate;
        foundInspection.DeathDate = inspectionEditModel.DeathDate;

        //_context.Update(foundInspection);
        _context.SaveChanges();
        return new object();
    }

    public IEnumerable<InspectionPreviewModel> GetInspectionChain(Guid id)
    {

        var foundInspection = _context.Inspection
            .Include(inspectionEntity => inspectionEntity.PreviousInspection)
            .Include(inspectionEntity => inspectionEntity.Diagnoses)
            .Include(inspectionEntity => inspectionEntity.Doctor)
            .Include(inspectionEntity => inspectionEntity.Patient)
            .FirstOrDefault(x => x.Id == id);

        if (foundInspection == null)
        {
            throw new NotFoundException($"Inspection with id={id} not found");
        }

        if (foundInspection.PreviousInspection != null)
        {
            throw new NotFoundException($"Try to get chain for non-root medical inspection with id={id}");
        }

        IEnumerable<InspectionEntity>? response = ArraySegment<InspectionEntity>.Empty;
        while (foundInspection != null)
        {
            response = response.Append(foundInspection);
            foundInspection = _context.Inspection
                .Include(inspectionEntity => inspectionEntity.PreviousInspection)
                .Include(inspectionEntity => inspectionEntity.Diagnoses)
                .Include(inspectionEntity => inspectionEntity.Doctor)
                .Include(inspectionEntity => inspectionEntity.Patient)
                .FirstOrDefault(x => x.PreviousInspection == foundInspection);
            /*response = _context.Inspection
                .Where(x => x == foundInspection)
                .Select(i => new InspectionPreviewModel
            {

            }).AsEnumerable();*/
        }

        var responseInspections = response.Select(i => new InspectionPreviewModel
        {
            Date = i.Date,
            Conclusion = i.Conclusion,
            PreviousId = i.PreviousInspection?.Id,
            Patient = i.Patient.Name,
            PatientId = i.Patient.Id,
            Doctor = i.Doctor.Name,
            DoctorId = i.Doctor.Id,
            Diagnosis = i.Diagnoses
                .Where(d => d.Type == DiagnosisTypeEnum.Main)
                .Select(d => new DiagnosisModel
            {
                Code = d.Code,
                Name = d.Name,
                Description = d.Description,
                Type = d.Type,
                Id = d.Id,
                CreateTime = d.CreateTime
            }).First(),
            Id = i.Id,
            CreateTime = i.CreateTime,
            HasChain = i.HasChain,
            HasNested = i.HasNested
        });

        return responseInspections;
        throw new NotImplementedException();
    }
}