using DataAccess.Models;

namespace DataAccess.Repository.Interface
{
    public interface IReportRepository
    {
        Task<bool> AddReport(Report report);

        Task<bool> ApproveReport(int reportId, int processedBy);

        Task<Report> GetReportById(int reportId);

        Task<bool> PayFine(int violationId);

        Task<IEnumerable<Violation>> GetViolationsByUserId(int userId);

        IEnumerable<Report> GetReportsByUserIdAndFilters(
            int userId,
            DateOnly? fromDate,
            DateOnly? toDate,
            string? status,
            string? violationType,
            string? plateNumber);
    }
}
