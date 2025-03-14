using DataAccess.Models;
using DataAccess.Repository.Interface;
using DataAccess.Repository;

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

        public Violation GetViolationById(int violationId)
        {
            try
            {
                return _violationRepository.GetViolationById(violationId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin vi phạm: {ex.Message}");
            }
        }

        public List<Violation> GetViolationsByViolatorId(int violatorId)
        {
            try
            {
                return _violationRepository.GetViolationsByViolatorId(violatorId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách vi phạm: {ex.Message}");
            }
        }

        public bool AddViolation(Violation violation)
        {
            try
            {
                return _violationRepository.AddViolation(violation);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm vi phạm: {ex.Message}");
            }
        }

        public bool UpdateViolation(Violation violation)
        {
            try
            {
                return _violationRepository.UpdateViolation(violation);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật vi phạm: {ex.Message}");
            }
        }

        public bool DeleteViolation(int violationId)
        {
            try
            {
                return _violationRepository.DeleteViolation(violationId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa vi phạm: {ex.Message}");
            }
        }

        public bool AddViolationResponse(int violationId, string response)
        {
            try
            {
                var violation = _violationRepository.GetViolationById(violationId);
                if (violation != null)
                {
                    if (violation.ResponseCount >= 3)
                    {
                        throw new Exception("Bạn đã đạt giới hạn số lần phản hồi (tối đa 3 lần).");
                    }
                    violation.Response = response;
                    violation.ResponseCount += 1;

                    _violationRepository.UpdateViolation(violation);

                    NotifyTrafficPolice(violation);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm phản hồi: {ex.Message}");
            }
        }

        private void NotifyTrafficPolice(Violation violation)
        {
            try
            {
                var trafficPolice = _context.Users.Where(u => u.Role == "TrafficPolice").ToList();

                foreach (var police in trafficPolice)
                {
                    var notification = new Notification
                    {
                        UserId = police.UserId,
                        Message = $"Có phản hồi mới từ người vi phạm biển số {violation.PlateNumber}: {violation.Response}",
                        PlateNumber = violation.PlateNumber,
                        SentDate = DateTime.Now
                    };
                    _context.Notifications.Add(notification);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thông báo cho công an: {ex.Message}");
            }
        }
    }
}
