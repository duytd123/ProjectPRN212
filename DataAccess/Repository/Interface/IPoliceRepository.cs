using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Interface
{
    public interface IPoliceRepository
    {
        List<Report> GetAllReports();
        void UpdateReportStatus(int reportId, string status, int processedBy);
        void SendNotification(int userId, string message, string plateNumber, decimal? fineAmount, DateTime? dueDate); 
        void VerifyAndProcessReport(int reportId, string status, int processedBy, string? rejectionReason = null);
        User? GetUserByPlateNumber(string plateNumber);
        bool HasNotificationBeenSent(int reportId);
        Violation? GetViolationByReportId(int reportId);
    }
}
