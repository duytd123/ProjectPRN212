using DataAccess.Models;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ProjectPrn212Context _context;

        public ReportRepository(ProjectPrn212Context context)
        {
            _context = context;
        }

        public async Task<bool> AddReport(Report report)
        {
            try
            {
                var context = new ProjectPrn212Context();
                var userExists = await context.Users.AnyAsync(u => u.UserId == report.ReporterId);

                if (!userExists)
                {
                    throw new Exception($"UserID {report.ReporterId} không tồn tại.");
                }
                context.Add(report);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                // Log lỗi chi tiết
                Console.WriteLine($"Error details: {e.InnerException?.Message ?? e.Message}");
                throw;
            }
        }

        public IEnumerable<Report> GetReportsByUserIdAndFilters(int userId, DateOnly? fromDate, DateOnly? toDate, string? status, string? violationType, string? plateNumber)
        {
            try
            {
                var query = _context.Reports.Where(r => r.ReporterId == userId);

                if (fromDate.HasValue)
                {
                    var fromDateTime = fromDate.Value.ToDateTime(TimeOnly.MinValue);
                    query = query.Where(r => r.ReportDate >= fromDateTime);
                }

                if (toDate.HasValue)
                {
                    var toDateTime = toDate.Value.ToDateTime(TimeOnly.MaxValue);
                    query = query.Where(r => r.ReportDate <= toDateTime);
                }

                if (!string.IsNullOrEmpty(status) && status != "Tất cả")
                {
                    query = query.Where(r => r.Status == status);
                }

                if (!string.IsNullOrEmpty(violationType) && violationType != "Tất cả")
                {
                    query = query.Where(r => r.ViolationType == violationType);
                }

                if (!string.IsNullOrEmpty(plateNumber))
                {
                    query = query.Where(r => r.PlateNumber.Contains(plateNumber));
                }

                return query
                    .Include(r => r.Reporter)
                    .OrderByDescending(r => r.ReportDate)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
