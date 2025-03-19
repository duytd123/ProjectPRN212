using DataAccess.Models;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public List<Violation> GetViolationsByUserId(int userId)
        {
            return _context.Violations
                .Include(v => v.ViolationType)
                .Include(v => v.Report)
                .Where(v => v.ViolatorId == userId)
                .ToList();
        }

        public Violation? GetViolationById(int violationId)
        {
            return _context.Violations
                .Include(v => v.ViolationType)
                .Include(v => v.Report)
                .FirstOrDefault(v => v.ViolationId == violationId);
        }

        public void UpdateViolation(Violation violation)
        {
            _context.Violations.Update(violation);
            _context.SaveChanges();
        }

        public void SubmitResponse(int violationId, string response)
        {
            var violation = GetViolationById(violationId);
            if (violation != null && violation.ResponseCount < 3)
            {
                violation.Response = response;
                violation.ResponseCount++;
                UpdateViolation(violation);

                // Chuyển trạng thái report về Pending
                var report = _context.Reports.FirstOrDefault(r => r.ReportId == violation.ReportId);
                if (report != null)
                {
                    report.Status = "Pending";
                    _context.SaveChanges();
                }
            }
            else
            {
                throw new InvalidOperationException("Bạn đã gửi phản hồi tối đa 3 lần.");
            }
        }
    }
}