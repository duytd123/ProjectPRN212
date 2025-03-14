using DataAccess.Models;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    public class ViolationRepository : IViolationRepository
    {
        private readonly ProjectPrn212Context _context;

        public ViolationRepository(ProjectPrn212Context context)
        {
            _context = context;
        }

        public Violation GetViolationById(int violationId)
        {
            return _context.Violations.Include(v => v.Report).Include(v => v.Violator).FirstOrDefault(v => v.ViolationId == violationId);
        }

        public List<Violation> GetViolationsByViolatorId(int violatorId)
        {
            return _context.Violations.Include(v => v.Report).Where(v => v.ViolatorId == violatorId).ToList();
        }

        public bool AddViolation(Violation violation)
        {
            try
            {
                _context.Violations.Add(violation);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateViolation(Violation violation)
        {
            try
            {
                _context.Violations.Update(violation);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteViolation(int violationId)
        {
            try
            {
                var violation = _context.Violations.Find(violationId);
                if (violation != null)
                {
                    _context.Violations.Remove(violation);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
