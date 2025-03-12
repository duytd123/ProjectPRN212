using DataAccess.Models;
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

        public ViolationObject(IViolationRepository violationRepository)
        {
            _violationRepository = violationRepository;
        }
        public List<Report> GetViolationReports()
        {
            return _violationRepository.GetViolationReports();
        }
        public Dictionary<string, int> GetReportStatistics()
        {
            return _violationRepository.GetReportStatistics();
        }
    }
}
