using DataAccess.Models;
using DataAccess.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class TrustedDeviceRepository : ITrustedDeviceRepository
    {
        private readonly ProjectPrn212Context _context;

        public TrustedDeviceRepository(ProjectPrn212Context context)
        {
            _context = context;
        }

        public bool IsDeviceTrusted(int userId, string deviceToken)
        {
            return _context.TrustedDevices.Any(td => td.UserId == userId && td.DeviceToken == deviceToken);
        }

        public void AddTrustedDevice(int userId, string deviceToken)
        {
            if (!IsDeviceTrusted(userId, deviceToken))
            {
                _context.TrustedDevices.Add(new TrustedDevice { UserId = userId, DeviceToken = deviceToken });
                _context.SaveChanges();
            }
        }
    }
}
