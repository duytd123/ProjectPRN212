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
                var userExists = await _context.Users.AnyAsync(u => u.UserId == report.ReporterId);

                if (!userExists)
                {
                    throw new Exception($"UserID {report.ReporterId} không tồn tại.");
                }
                _context.Add(report);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error details: {e.InnerException?.Message ?? e.Message}");
                throw;
            }
        }

        public Report GetReportByPlateNumber(string plateNumber)
        {
            return _context.Reports
                .Include(r => r.Violations)
                .FirstOrDefault(r => r.PlateNumber == plateNumber);
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
