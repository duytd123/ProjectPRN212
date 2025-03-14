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

        public async Task<Report> GetReportById(int reportId)
        {
            return await _context.Reports.Include(r => r.Reporter).FirstOrDefaultAsync(r => r.ReportId == reportId);
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

        public async Task<IEnumerable<Violation>> GetViolationsByUserId(int userId)
        {
            return await _context.Violations
                .Include(v => v.Report)
                .Where(v => v.ViolatorId == userId)
                .ToListAsync();
        }

        public async Task<bool> ApproveReport(int reportId, int processedBy)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report != null && report.Status == "Pending")
            {
                report.Status = "Approved";
                report.ProcessedBy = processedBy;
                await _context.SaveChangesAsync();

                // Gửi thông báo cho người vi phạm
                var violator = await _context.Users.FirstOrDefaultAsync(u => u.Vehicles.Any(v => v.PlateNumber == report.PlateNumber));
                if (violator != null)
                {
                    var notification = new Notification
                    {
                        UserId = violator.UserId,
                        Message = $"Bạn đã nhận được phản ánh vi phạm với biển số {report.PlateNumber}.",
                        PlateNumber = report.PlateNumber,
                        SentDate = DateTime.Now
                    };
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            return false;
        }

        public async Task<bool> PayFine(int violationId)
        {
            var violation = await _context.Violations.FindAsync(violationId);
            if (violation != null)
            {
                violation.PaidStatus = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
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
