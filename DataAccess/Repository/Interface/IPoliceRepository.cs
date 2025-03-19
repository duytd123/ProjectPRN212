using DataAccess.Models;

namespace DataAccess.Repository.Interface
{
    public interface IPoliceRepository
    {
        List<Report> GetAllReports();
        int? GetPoliceUserId(string email);
        void UpdateReportStatus(int reportId, string status, int processedBy);
        void SendNotification(int userId, string message, string plateNumber, decimal? fineAmount, DateTime? dueDate);
        User? GetUserByPlateNumber(string plateNumber);
        void VerifyAndProcessReport(int reportId, string status, int processedBy, string? rejectionReason = null);
        bool HasNotificationBeenSent(int reportId);
        Violation? GetViolationByReportId(int reportId);
        bool DoesVehicleExist(string plateNumber);
        void AddViolation(Violation violation);
        Report GetReportById(int reportId);
        List<string> GetUserVehiclePlateNumbers(int userId);
        List<Report> GetViolationsByUserId(int userId);
    }
}
