using MedicalInformationSystem.Data;
using MedicalInformationSystem.Data.Entities;
using MedicalInformationSystem.Exceptions;
using MedicalInformationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalInformationSystem.Services.ConsultationService;

public class ConsultationService : IConsultationService
{
    private readonly AppDbContext _context;

    public ConsultationService(AppDbContext context)
    {
        _context = context;
    }
    
    public InspectionPagedListModel GetInspectionsWithConsultations(Guid doctorId, bool grouped, Guid[] icdRoots, int page,
        int size)
    {
        foreach (var icdRoot in icdRoots)
        {
            if (!_context.IcdRoots.Any(x => x.NewId == icdRoot))
            {
                throw new BadRequest("Invalid argument for subsets");
            }
        }
        var doctorSpeciality = _context.Doctor
            .Include(doctorEntity => doctorEntity.Speciality)
            .First(x => x.Id == doctorId).Speciality;
        var inspections = _context.Inspection
            .Include(y => y.Consultations)
            .Where(x => x.Consultations != null && x.Consultations
                .Any(z => z.Speciality == doctorSpeciality));
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
        return new InspectionPagedListModel
        {
            Inspections = responseInspections.Select(i => new InspectionPreviewModel
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
            }),
            Pagination = pagination
        };
    }

    public ConsultationModel GetConsultation(Guid id)
    {
        var consultation = _context.Consultation.Where(x => x.Id == id);
        if (!consultation.Any())
        {
            throw new NotFoundException($"Consultation with id={id} not found in  database");
        }

        var consultationsResponse = consultation.Select(c => new ConsultationModel
        {
            Id = c.Id,
            CreateTime = c.CreateTime,
            InspectionId = c.Inspection.Id,
            Speciality = new SpecialityModel
            {
                Id = c.Speciality.Id,
                CreateTime = c.Speciality.CreateTime,
                Name = c.Speciality.Name
            },
            Comments = c.Comment.Select(comment => new CommentModel
            {
                Id = comment.Id,
                CreateTime = comment.CreateTime,
                ModifiedDate = comment.ModifiedDate,
                Content = comment.Content,
                AuthorId = comment.Author.Id,
                Author = comment.Author.Name,
                ParentId = comment.Parent == null ? null : comment.Parent.Id
            })
        }).First();

        return consultationsResponse; 
    }

    public Guid AddCommentToConsultation(Guid doctorId, Guid id, CommentCreateModel commentCreateModel)
    {
        var foundConsultation = _context.Consultation
            .Include(consultationEntity => consultationEntity.Speciality)
            .Include(consultationEntity => consultationEntity.Inspection)
            .ThenInclude(inspectionEntity => inspectionEntity.Doctor)
            .FirstOrDefault(x => x.Id == id);

        if (foundConsultation == null)
        {
            throw new NotFoundException("Consultation not found in  database");
        }

        var parentComment = _context.Comment
            .Include(commentEntity => commentEntity.Consultation)
            .FirstOrDefault(x => x.Id == commentCreateModel.ParentId);
        if (parentComment == null)
        {
            throw new NotFoundException("Parent comment not found in  database");
        }

        if (parentComment.Consultation != foundConsultation)
        {
            throw new BadRequest("Parent comment doesn't belong to this consultation");
        }
        
        var doctorSpeciality = _context.Doctor
            .Include(doctorEntity => doctorEntity.Speciality)
            .First(x => x.Id == doctorId).Speciality;

        if (doctorSpeciality != foundConsultation.Speciality)
        {
            if (foundConsultation.Inspection.Doctor.Id != doctorId)
            {
                throw new Forbidden(
                    "User doesn't have add comment to consultation (unsuitable specialty or not the inspection author)");
            }
        }

        var newComment = new CommentEntity
        {
            Id = Guid.NewGuid(),
            Content = commentCreateModel.Content,
            Author = foundConsultation.Inspection.Doctor,
            Consultation = foundConsultation,
            ModifiedDate = null,
            CreateTime = DateTime.Now.ToUniversalTime(),
            Parent = parentComment
        };

        _context.Comment.Add(newComment);
        _context.SaveChanges();

        return newComment.Id;
    }

    public void EditComment(Guid doctorId, Guid id, InspectionCommentCreateModel inspectionCommentCreateModel)
    {
        var foundComment = _context.Comment
            .Include(commentEntity => commentEntity.Author)
            .FirstOrDefault(x => x.Id == id);

        if (foundComment == null)
        {
            throw new NotFoundException($"Comment with id={id} not found in  database");
        }

        if (foundComment.Author.Id != doctorId)
        {
            throw new Forbidden($"The user with id={doctorId} is not the author of the comment");
        }

        foundComment.Content = inspectionCommentCreateModel.Content;
        foundComment.ModifiedDate = DateTime.Now.ToUniversalTime();
        _context.SaveChanges();
    }
}