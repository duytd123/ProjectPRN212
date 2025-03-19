using DataAccess.Models;
using System;
using System.Collections.Generic;
using DataAccess.Repository.Interface;

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

        public int? GetLoggedInPoliceUserId(string email)
        {
            return _policeRepository.GetPoliceUserId(email);
        }

        public void VerifyAndProcessReport(int reportId, string status, int processedBy, string? rejectionReason = null)
        {
            _policeRepository.VerifyAndProcessReport(reportId, status, processedBy, rejectionReason);
        }

        public void NotifyViolator(int userId, string message, string plateNumber, decimal? fineAmount, DateTime? dueDate)
        {
            _policeRepository.SendNotification(userId, message, plateNumber, fineAmount, dueDate);
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

        public void ProcessResponse(int violationId, bool isApproved)
        {
            _policeRepository.ProcessResponse(violationId, isApproved);
        }
    }
}
