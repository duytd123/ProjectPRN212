using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class ViolationObject
    {
        private readonly IViolationRepository _violationRepository;
        private readonly ProjectPrn212Context _context;

        public ViolationObject()
        {
            _context = new ProjectPrn212Context();
            _violationRepository = new ViolationRepository(_context);
        }

        public List<Report> GetViolationReports()
        {
            return _violationRepository.GetViolationReports();
        }

        public Dictionary<string, int> GetReportStatistics()
        {
            return _violationRepository.GetReportStatistics();
        }

        public List<Violation> GetViolationsByType(int violationTypeId)
        {
            return _violationRepository.GetViolationsByType(violationTypeId);
        }

        public async Task<List<ViolationType>> GetAllViolationTypes()
        {
            return await _violationRepository.GetAllViolationTypes();
        }

        public List<Violation> GetViolationsByUserId(int userId)
        {
            return _violationRepository.GetViolationsByUserId(userId);
        }

        public void SubmitResponse(int violationId, string response)
        {
            _violationRepository.SubmitResponse(violationId, response);
        }

        public Violation? GetViolationById(int violationId)
        {
            return _violationRepository.GetViolationById(violationId);
        }

        public void UpdateViolation(Violation violation)
        {
            _violationRepository.UpdateViolation(violation);
        }

        public void ProcessPayment(int violationId)
        {
            var violation = _violationRepository.GetViolationById(violationId);
            if (violation != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == violation.ViolatorId);
                if (user != null)
                {
                    if (user.Balance >= violation.ViolationType.FineAmount)
                    {
                        user.Balance -= violation.ViolationType.FineAmount;
                        violation.PaidStatus = true;
                        _violationRepository.UpdateViolation(violation);
                        _context.SaveChanges();
                    }
                    else
                    {
                        throw new InvalidOperationException("Số dư không đủ để thanh toán.");
                    }
                }
            }
        }
    }
}