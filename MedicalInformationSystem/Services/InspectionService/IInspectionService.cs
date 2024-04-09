using MedicalInformationSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalInformationSystem.Services.InspectionService;

public interface IInspectionService
{ 
    InspectionModel GetInspectionInformation([FromQuery] Guid id);
    
    object? EditInspectionInformation(Guid id,
        Guid doctorId,
        InspectionEditModel inspectionEditModel);
    
    IEnumerable<InspectionPreviewModel> GetInspectionChain(Guid id);
}