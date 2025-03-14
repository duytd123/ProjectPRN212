using DataAccess.Models;

namespace DataAccess.Repository.Interface
{
    public interface IReportRepository
    {
        Task<bool> AddReport(Report report);

        IEnumerable<Report> GetReportsByUserIdAndFilters(
            int userId,
            DateOnly? fromDate,
            DateOnly? toDate,
            string? status,
            string? violationType,
            string? plateNumber);
    }
}
