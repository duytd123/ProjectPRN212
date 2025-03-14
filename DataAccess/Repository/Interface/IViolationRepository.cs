using DataAccess.Models;

namespace DataAccess.Repository.Interface
{
    public interface IViolationRepository
    {
        Violation GetViolationById(int violationId);
        List<Violation> GetViolationsByViolatorId(int violatorId);
        bool AddViolation(Violation violation);
        bool UpdateViolation(Violation violation);
        bool DeleteViolation(int violationId);
    }
}
