using MedicalInformationSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalInformationSystem.Services.PatientService;

public interface IPatientService
{
    public Guid CreatePatient(
        PatientCreateModel patientCreateModel 
        );
    
    public PatientPagedListModel GetPatients(
        Guid doctorId, 
        string? name,
        List<ConclusionEnum> conclusions,
        PatientSortingEnum sorting,
        bool scheduledVisits,
        bool onlyMine,
        int page,
        int size);

    public Guid CreateInspection(
        Guid doctorId,
        Guid patientId,
        InspectionCreateModel inspectionCreateModel);

    public InspectionPagedListModel GetPatientInspections(
        Guid doctorId,
        Guid patientId,
        Guid[] icdRoots,
        bool grouped,
        int page, 
        int size);

    public PatientModel GetPatientCard(Guid patientId);

    public IEnumerable<InspectionShortModel> GetInspectionsWithoutChild(Guid patientId,
        string? request);
}