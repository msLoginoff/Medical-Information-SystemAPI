using System.ComponentModel.DataAnnotations;

namespace MedicalInformationSystem.Models;

public class InspectionPagedListModel
{
    public IEnumerable<InspectionPreviewModel>? Inspections { get; set; }
    public PageInfoModel Pagination { get; set; }
}