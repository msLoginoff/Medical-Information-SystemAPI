using System.Text.RegularExpressions;
using MedicalInformationSystem.Data;
using MedicalInformationSystem.Data.Entities;
using MedicalInformationSystem.Exceptions;
using MedicalInformationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalInformationSystem.Services.PatientService;

public class PatientService : IPatientService
{
    private readonly AppDbContext _context;

    public PatientService(AppDbContext context)
    {
        _context = context;
    }

    public Guid CreatePatient(PatientCreateModel patientCreateModel)
    {
        if (patientCreateModel.BirthDateTime > DateTime.Now)
        {
            throw new BadRequest("invalid birthdate");
        }

        var patientId = Guid.NewGuid();

        var newPatient = new PatientEntity
        {
            Id = patientId,
            CreateTime = DateTime.Now.ToUniversalTime(),
            Name = patientCreateModel.Name,
            BirthDateTime = patientCreateModel.BirthDateTime.ToUniversalTime(),
            Gender = patientCreateModel.Gender
        };

        _context.Patient.Add(newPatient);
        _context.SaveChanges();

        return patientId;
    }

    public PatientPagedListModel GetPatients(Guid doctorId, string? name, List<ConclusionEnum> conclusions,
        PatientSortingEnum sorting,
        bool scheduledVisits, bool onlyMine, int page, int size)
    {
        var patients = _context.Patient.AsQueryable();

        if (name != null && patients.Any())
        {
            patients = patients.Where(x => x.Name.Contains(name));
        }

        if (conclusions.Any())
        {
            patients = patients
                .Where(patient => patient.Inspection != null && patient.Inspection
                    .Any(x => conclusions
                        .Contains(x.Conclusion)));
        }

        if (scheduledVisits)
        {
            patients = patients
                .Where(patient => patient.Inspection != null && patient.Inspection
                    .Any(x => x.NextVisitDate != null));
        }

        if (onlyMine)
        {
            patients = patients
                .Where(patient => patient.Inspection != null && patient.Inspection
                    .Any(inspection => inspection.Doctor.Id == doctorId));
        }

        patients = sorting switch
        {
            PatientSortingEnum.NameAsc => patients.OrderBy(x => x.Name),
            PatientSortingEnum.NameDesc => patients.OrderByDescending(x => x.Name),
            PatientSortingEnum.CreateAsc => patients.OrderBy(x => x.CreateTime),
            PatientSortingEnum.CreateDesc => patients.OrderByDescending(x => x.CreateTime),
            PatientSortingEnum.InspectionAsc => patients.OrderBy(x => x.Inspection.OrderBy(y => y.Date).First()), 
            PatientSortingEnum.InspectionDesc => patients.OrderByDescending(x => x.Inspection.OrderBy(y => y.Date).First()), //todo add sorting 
            _ => throw new ArgumentOutOfRangeException(nameof(sorting), sorting, null)
        };

        var pagination = new PageInfoModel(size, patients.Count(), page);

        var patientsRequest = patients.Skip((page - 1) * size).Take(size).ToList();

        var request = new PatientPagedListModel
        {
            Patients = patientsRequest.Select(n => new PatientModel
            {
                Id = n.Id,
                CreateTime = n.CreateTime,
                Name = n.Name,
                BirthDateTime = n.BirthDateTime,
                Gender = n.Gender
            }),

            Pagination = pagination
        };

        return request;
    }

    public Guid CreateInspection(Guid doctorId, Guid patientId, InspectionCreateModel inspectionCreateModel)
    {
        if (inspectionCreateModel.Date > DateTime.Now)
        {
            throw new BadRequest($"Inspection date and time can't be later than now {DateTime.Now}");
        }

        if (inspectionCreateModel.Diagnoses
                .Count(x => x.Type == DiagnosisTypeEnum.Main) != 1)
        {
            throw new BadRequest($"Inspection always must contain one diagnosis with Type equal to Main");
        }

        if (inspectionCreateModel.Conclusion == ConclusionEnum.Disease)
        {
            if (inspectionCreateModel.NextVisitDate == null)
            {
                throw new BadRequest($"Conclusion Disease requires not null correct date");
            }

            if (inspectionCreateModel.NextVisitDate <= inspectionCreateModel.Date)
            {
                throw new BadRequest($"Next visit date can't be earlier this inspection date");
            }

            if (inspectionCreateModel.DeathDate != null)
            {
                throw new BadRequest($"Conclusion Disease requires null Death date");
            }
        }
        

        if (inspectionCreateModel.Conclusion == ConclusionEnum.Death)
        {
            if (inspectionCreateModel.DeathDate == null)
            {
                throw new BadRequest($"Conclusion Death requires not null correct date and time");
            }

            if (inspectionCreateModel.DeathDate > DateTime.Now)
            {
                throw new BadRequest($"Conclusion Death requires correct date and time, which can't be later than now {DateTime.Now}");
            }

            if (inspectionCreateModel.DeathDate <= inspectionCreateModel.Date)
            {
                throw new BadRequest($"Death date can't be earlier this inspection date");
            }

            if (inspectionCreateModel.NextVisitDate != null)
            {
                throw new BadRequest($"Conclusion Death requires null next visit date");
            }
        }

        if (inspectionCreateModel.Conclusion == ConclusionEnum.Recovery &&
            (inspectionCreateModel.NextVisitDate != null ||
             inspectionCreateModel.DeathDate != null))
        {
            throw new BadRequest("Death date and Next Visit date must be empty in case of Recovery conclusion");
        }
        
        var patient = _context.Patient.FirstOrDefault(x => x.Id == patientId);

        if (patient == null)
        {
            throw new NotFoundException($"Patient '{patientId}' not found");
        }

        if (_context.Inspection.Any(x => x.Conclusion == ConclusionEnum.Death &&
                                         x.Patient.Id == patientId))
        {
            throw new BadRequest(
                $"This patient died");
        }

        var newInspection = new InspectionEntity();

        if (inspectionCreateModel.PreviousInspectionId != null)
        {
            if (inspectionCreateModel.Date < _context.Inspection
                    .First(x => x.Id == inspectionCreateModel.PreviousInspectionId)
                    .Date)
            {
                throw new BadRequest(
                    $"Inspection date and time can't be earlier than date and time of previous inspection");
            }

            

            var previousInspection = _context.Inspection
                .Include(inspectionEntity => inspectionEntity.PreviousInspection)
                .FirstOrDefault(x =>
                x.Id == inspectionCreateModel.PreviousInspectionId && x.Patient.Id == patientId);
            
            if (previousInspection == null)
            {
                throw new NotFoundException("Previous inspection not found");
            }

            if (previousInspection.HasNested)
            {
                throw new BadRequest($"Child inspection already exists for inspection {previousInspection.Id}");
            }

            newInspection.PreviousInspection = previousInspection;
            previousInspection.HasNested = true;
            previousInspection.NextInspection = newInspection;

            if (previousInspection.PreviousInspection == null)
            {
                previousInspection.HasChain = true;
            }
        }
        else
        {
            newInspection.PreviousInspection = null;
        }

        
        newInspection.Id = Guid.NewGuid();
        newInspection.CreateTime = DateTime.Now.ToUniversalTime();
        newInspection.Date = inspectionCreateModel.Date;
        newInspection.Anamnesis = inspectionCreateModel.Anamnesis;
        newInspection.Complaints = inspectionCreateModel.Complaints;
        newInspection.Treatment = inspectionCreateModel.Treatment;
        newInspection.Conclusion = inspectionCreateModel.Conclusion;
        newInspection.NextVisitDate = inspectionCreateModel.NextVisitDate;
        newInspection.DeathDate = inspectionCreateModel.DeathDate;
        newInspection.HasNested = false;
        newInspection.HasChain = false;
        newInspection.Patient = patient;
        newInspection.NextInspection = null;
        newInspection.Doctor = _context.Doctor
            .Include(doctorEntity => doctorEntity.Speciality)
            .First(x => x.Id == doctorId);

        foreach (var diagnosis in inspectionCreateModel.Diagnoses)
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
                Inspection = newInspection,
                Name = icd.MKB_NAME,
                Type = diagnosis.Type
            };
            _context.Add(newDiagnosis);
        }
        
        var consultations = inspectionCreateModel.Consultations.ToList();
            
        if (consultations.FirstOrDefault() != null)
        {
            
            IEnumerable<Guid> used = new[] { newInspection.Doctor.Speciality.Id };
            foreach (var consultation in consultations)
            {
                if (used.Contains(consultation.SpecialityId))
                {
                    throw new BadRequest("Inspection cannot have several consultations with the same specialty of a doctor");
                }

                used = used.Append(consultation.SpecialityId);
                if (!_context.Speciality.Any(x => x.Id == consultation.SpecialityId))
                {
                    throw new BadRequest("Speciality not found");
                }

                var speciality = _context.Speciality.First(x => x.Id == consultation.SpecialityId);

                var newConsultation = new ConsultationEntity
                {
                    Speciality = speciality,
                    CreateTime = DateTime.Now.ToUniversalTime(),
                    Inspection = newInspection,
                    Id = Guid.NewGuid(),
                };
                _context.Add(newConsultation);

                var newComment = new CommentEntity
                {
                    Author = newInspection.Doctor,
                    Consultation = newConsultation,
                    Id = Guid.NewGuid(),
                    CreateTime = DateTime.Now.ToUniversalTime(),
                    ModifiedDate = null,
                    Content = consultation.Comment.Content,
                    Parent = null,
                    NestedComment = null
                };

                _context.Add(newComment);
            }
        }

        _context.Add(newInspection);
        _context.SaveChanges();
        
        //todo add consultations

        return newInspection.Id;
    }

    public InspectionPagedListModel GetPatientInspections(Guid doctorId,
        Guid patientId,
        Guid[] icdRoots,
        bool grouped,
        int page,
        int size)
    {
        var inspections = _context.Inspection.Where(x => x.Patient.Id == patientId);

        if (icdRoots.Any())
        {
            inspections = inspections
                .Where(x => x.Diagnoses
                    .Any(y => y.Type == DiagnosisTypeEnum.Main && icdRoots
                        .Contains(y.IcdDiagnosis.IcdRoot.NewId)));
        }

        if (grouped)
        {
            inspections = inspections.Where(x => x.PreviousInspection == null);
        }

        var pagination = new PageInfoModel(size, inspections.Count(), page);
        var responseInspections = inspections
            .Skip((page - 1) * size)
            .Take(size)
            .Include(doctor => doctor.Doctor)
            .Include(patient => patient.Patient)
            .Include(diagnosis => diagnosis.Diagnoses)
            .ToList();
        var response = new InspectionPagedListModel
        {
            Inspections = responseInspections.Select(i => new InspectionPreviewModel
            {
                Id = i.Id,
                CreateTime = i.CreateTime,
                PreviousId = i.PreviousInspection?.Id,
                Date = i.Date,
                Conclusion = i.Conclusion,
                DoctorId = i.Doctor.Id,
                Doctor = i.Doctor.Name,
                PatientId = i.Patient.Id,
                Patient = i.Patient.Name,
                /*Diagnosis = new DiagnosisModel
                {
                    Id = i.Diagnoses.First(x => x.Type == DiagnosisTypeEnum.Main).Id,
                    CreateTime = i.Diagnoses.First(x => x.Type == DiagnosisTypeEnum.Main).CreateTime,
                    Code = i.Diagnoses.First(x => x.Type == DiagnosisTypeEnum.Main).Code,
                    Description = i.Diagnoses.First(x => x.Type == DiagnosisTypeEnum.Main).Description,
                    Name = i.Diagnoses.First(x => x.Type == DiagnosisTypeEnum.Main).Name,
                    Type = i.Diagnoses.First(x => x.Type == DiagnosisTypeEnum.Main).Type
                }*/
                Diagnosis = i.Diagnoses.Where(d => d.Type == DiagnosisTypeEnum.Main).Select(
                    d => new DiagnosisModel
                {
                    Id = d.Id,
                    CreateTime = d.CreateTime,
                    Code = d.Code,
                    Description = d.Description,
                    Name = d.Name,
                    Type = d.Type
                }).First(),
                HasChain = i.HasChain,
                HasNested = i.HasNested
            }),
            Pagination = pagination
        };

        return response;
    }

    public PatientModel GetPatientCard(Guid patientId)
    {
        var patients = _context.Patient.FirstOrDefault(x => x.Id == patientId);

        if (patients == null)
        {
            throw new BadRequest($"Patient with id={patientId} not found in database");
        }

        return new PatientModel
        {
            Id = patients.Id,
            CreateTime = patients.CreateTime,
            Name = patients.Name,
            BirthDateTime = patients.BirthDateTime.ToUniversalTime(),
            Gender = patients.Gender
        };
    }

    public IEnumerable<InspectionShortModel> GetInspectionsWithoutChild(Guid patientId, string? request)
    {
        if (!_context.Patient.Any(x => x.Id == patientId))
        {
            throw new BadRequest($"Patient with id={patientId} not found in database");
        }
        var inspections = _context.Inspection
            .Where(x => x.Patient.Id == patientId && !x.HasNested);

        //var regex = new Regex("([A-Za-z][0-9]{2}(-[A-Za-z][0-9]{2}){0,1})||([A-Za-z][0-9]{2}/.[0-9])");
        if (request != null)
        {
            var inspectionsByName = inspections
                .Where(x => x.Diagnoses
                    .Any(d => d.Type == DiagnosisTypeEnum.Main && d.Name.Contains(request)));
            var inspectionsByCode= inspections
                .Where(x => x.Diagnoses
                    .Any(d => d.Type == DiagnosisTypeEnum.Main && d.Code.Contains(request)));
            inspections = inspectionsByName.Concat(inspectionsByCode);
        }

        inspections = inspections.Include(x => x.Diagnoses);
        var response = inspections.Select(i => new InspectionShortModel
        {
            Id = i.Id,
            CreateTime = i.CreateTime,
            Date = i.Date,
            Diagnosis = i.Diagnoses.Where(d => d.Type == DiagnosisTypeEnum.Main).Select(
                d => new DiagnosisModel
                {
                    Id = d.Id,
                    CreateTime = d.CreateTime,
                    Code = d.Code,
                    Description = d.Description,
                    Name = d.Name,
                    Type = d.Type
                }).First(),
        });

        return response;
    }
}