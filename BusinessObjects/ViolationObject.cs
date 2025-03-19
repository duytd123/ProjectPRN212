using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
