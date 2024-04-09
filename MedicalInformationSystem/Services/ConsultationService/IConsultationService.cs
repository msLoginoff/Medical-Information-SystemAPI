using MedicalInformationSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalInformationSystem.Services.ConsultationService;

public interface IConsultationService
{
    InspectionPagedListModel GetInspectionsWithConsultations(Guid doctorId,
        [FromQuery] bool grouped,
        Guid[] icdRoots,
        int page,
        int size);

    ConsultationModel GetConsultation(Guid id);

    Guid AddCommentToConsultation(Guid doctorId,
        [FromQuery] Guid id,
        [FromBody] CommentCreateModel commentCreateModel);
    
    void EditComment(Guid doctorId,
        [FromQuery] Guid id,
        [FromBody] InspectionCommentCreateModel inspectionCommentCreateModel);
}