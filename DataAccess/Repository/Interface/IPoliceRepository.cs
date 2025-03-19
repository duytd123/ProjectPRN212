using DataAccess.Models;

namespace DataAccess.Repository.Interface
{
    public interface IPoliceRepository
    {
        List<Report> GetAllReports();
        void SendNotification(int userId, string message, string plateNumber, decimal? fineAmount, DateTime? dueDate);
        void VerifyAndProcessReport(int reportId, string status, int processedBy, string? rejectionReason = null);
        User? GetUserByPlateNumber(string plateNumber);
        bool HasNotificationBeenSent(int reportId);
        Violation? GetViolationByReportId(int reportId);

        bool DoesVehicleExist(string plateNumber);
        int? GetPoliceUserId(string email);
        void ProcessResponse(int violationId, bool isApproved);
    }
}
