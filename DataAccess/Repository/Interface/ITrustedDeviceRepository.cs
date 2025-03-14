using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Interface
{
    public interface ITrustedDeviceRepository
    {
        bool IsDeviceTrusted(int userId, string deviceToken);
        void AddTrustedDevice(int userId, string deviceToken);
    }
}
