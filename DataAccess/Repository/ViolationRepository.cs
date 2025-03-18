using DataAccess.Models;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ViolationRepository : IViolationRepository
    {
        private readonly ProjectPrn212Context _context;

        public ViolationRepository(ProjectPrn212Context context)
        {
            _context = context;
        }

        public List<Report> GetViolationReports()
        {
            return _context.Reports
                .Include(r => r.Violations) 
                .Include(r => r.ViolationType)
                .Include(r => r.Reporter)
                .Include(r => r.ProcessedByNavigation)
                .ToList();
        }

        public List<Violation> GetViolationsByType(int violationTypeId)
        {
            return _context.Violations
                .Include(v => v.ViolationType)
                .Include(v => v.Report)
                .Where(v => v.ViolationTypeId == violationTypeId)
                .ToList();
        }
        public Dictionary<string, int> GetReportStatistics()
        {
            return _context.Reports
                .GroupBy(r => r.Status)
                .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());
        }

        public async Task<List<ViolationType>> GetAllViolationTypes()
        {
            return await _context.ViolationTypes.ToListAsync();
        }
    }
}
