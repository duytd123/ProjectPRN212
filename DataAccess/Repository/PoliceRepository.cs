using DataAccess.Models;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class PoliceRepository : IPoliceRepository
    {
        private readonly ProjectPrn212Context _context;

        public PoliceRepository(ProjectPrn212Context context)
        {
            _context = context;
        }

        public List<Report> GetAllReports()
        {
            return _context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.ProcessedByNavigation)
                .Include(r => r.ViolationType)
                .Include(r => r.Violations)
                .ToList();
        }

        public int? GetPoliceUserId(string email)
        {
            return _context.Users
                .Where(u => u.Email == email && u.Role == "TrafficPolice")
                .Select(u => u.UserId)
                .FirstOrDefault();
        }

        public void VerifyAndProcessReport(int reportId, string status, int processedBy, string? rejectionReason = null)
        {
            var validStatuses = new List<string> { "Pending", "Approved", "Rejected" };

            if (!validStatuses.Contains(status))
            {
                throw new ArgumentException($"Invalid status: {status}. Allowed values: {string.Join(", ", validStatuses)}");
            }

            var report = _context.Reports
                .Include(r => r.ViolationType)
                .Include(r => r.Violations)
                .FirstOrDefault(r => r.ReportId == reportId);

            if (report != null)
            {
                report.Status = status;
                report.ProcessedBy = processedBy;
                report.RejectionReason = rejectionReason;

                if (status == "Approved")
                {
                    var violation = report.Violations.FirstOrDefault();
                    if (violation != null)
                    {
                        // Xóa phản hồi của người vi phạm
                        violation.Response = null;
                        violation.ResponseCount = 0;
                        _context.Violations.Update(violation);

                        // Gửi thông báo đến người gửi đơn và người vi phạm
                        SendNotification(report.ReporterId, "Đơn phản ánh của bạn đã được duyệt.", report.PlateNumber, null, null);
                        if (violation.ViolatorId.HasValue)
                        {
                            SendNotification(violation.ViolatorId.Value, $"Bạn có biển số xe {report.PlateNumber} đã vi phạm với số tiền phạt {report.ViolationType.FineAmount} VND.", report.PlateNumber, report.ViolationType.FineAmount, DateTime.Now.AddDays(7));
                        }
                    }
                }

                _context.SaveChanges();
            }
        }

        public void SendNotification(int userId, string message, string plateNumber, decimal? fineAmount, DateTime? dueDate)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                PlateNumber = plateNumber,
                SentDate = DateTime.Now,
                IsRead = false
            };

            if (fineAmount.HasValue && dueDate.HasValue)
            {
                notification.Message += $"\n\nFine: {fineAmount.Value} VND\nDue Date: {dueDate.Value:dd/MM/yyyy}";
            }

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public User? GetUserByPlateNumber(string plateNumber)
        {
            return _context.Vehicles
                .Where(v => v.PlateNumber == plateNumber)
                .Select(v => v.Owner)
                .FirstOrDefault();
        }

        public bool HasNotificationBeenSent(int reportId)
        {
            var plateNumber = _context.Reports
                .Where(r => r.ReportId == reportId)
                .Select(r => r.PlateNumber)
                .FirstOrDefault();

            return _context.Notifications
                .Any(n => n.PlateNumber == plateNumber);
        }

        public Violation? GetViolationByReportId(int reportId)
        {
            return _context.Violations
                           .Include(v => v.ViolationType)
                           .FirstOrDefault(v => v.ReportId == reportId);
        }

        public bool DoesVehicleExist(string plateNumber)
        {
            return _context.Vehicles.Any(v => v.PlateNumber == plateNumber);
        }

        public void ProcessResponse(int violationId, bool isApproved)
        {
            var violation = _context.Violations.FirstOrDefault(v => v.ViolationId == violationId);
            if (violation != null)
            {
                if (isApproved)
                {
                    violation.PaidStatus = true;
                }
                else
                {
                    violation.IsResponseRejected = true;
                }
                _context.SaveChanges();
            }
        }
    }
}

