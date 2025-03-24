using DataAccess.Models;

namespace DataAccess.Repository.Interface
{
    public interface IPoliceRepository
    {
        IQueryable<Report> GetAllReports();
        void UpdateReportStatus(int reportId, string status, int processedBy);
        void SendNotification(int userId, string message, string plateNumber, decimal? fineAmount, DateTime? dueDate);
        void VerifyAndProcessReport(int reportId, string status, int processedBy, string? rejectionReason = null);
        User? GetUserByPlateNumber(string plateNumber);
        bool HasNotificationBeenSent(int reportId);
        Violation? GetViolationByReportId(int reportId);
        void ProcessViolationResponse(int violationId, string status, string rejectionReason = null);
        bool DoesVehicleExist(string plateNumber);
        int? GetPoliceUserId(string email);
    }
}
