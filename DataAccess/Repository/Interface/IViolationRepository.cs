using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Interface
{
    public interface IViolationRepository
    {
        List<Report> GetViolationReports(); 
        Dictionary<string, int> GetReportStatistics();
        List<Violation> GetViolationsByType(int violationTypeId);

        Task<List<ViolationType>> GetAllViolationTypes();
        Violation GetViolationById(int violationId);
        void UpdateViolationResponse(int violationId, string response);
    }
}
