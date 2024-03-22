namespace MedicalInformationSystem.Models;

public class IcdRootsReportModel
{
    public IcdRootsReportFiltersModel Filters { get; set; }
    public IEnumerable<IcdRootsReportRecordModel>? Records { get; set; }
    public Dictionary<string, int> SummaryByRoot { get; set; }
}