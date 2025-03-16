using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Repository.Interface;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects
{
    public class PoliceObject
    {
        private readonly IPoliceRepository _policeRepository;

        public PoliceObject(IPoliceRepository policeRepository)
        {
            _policeRepository = policeRepository;
        }

        public List<Report> GetAllReports()
        {
            return _policeRepository.GetAllReports();
        }

        public void VerifyAndProcessReport(int reportId, string status, int processedBy)
        {
            _policeRepository.UpdateReportStatus(reportId, status, processedBy);
        }

        public void NotifyViolator(int userId, string message, string plateNumber, decimal? fineAmount, DateTime? dueDate)
        {
            _policeRepository.SendNotification(userId, message, plateNumber, fineAmount, dueDate);
        }

        public void VerifyAndProcessReport(int reportId, string status, int processedBy, string? rejectionReason = null)
        {
            _policeRepository.VerifyAndProcessReport(reportId, status, processedBy, rejectionReason);
        }

        public User? GetUserByPlateNumber(string plateNumber)
        {
            return _policeRepository.GetUserByPlateNumber(plateNumber);
        }

        public bool HasNotificationBeenSent(int reportId)
        {
            return _policeRepository.HasNotificationBeenSent(reportId);
        }

        public Violation? GetViolationByReportId(int reportId)
        {
            return _policeRepository.GetViolationByReportId(reportId);
        }
        public bool DoesVehicleExist(string plateNumber)
        {
            return _policeRepository.DoesVehicleExist(plateNumber);
        }


    }
}
