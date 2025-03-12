using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Interface
{
    public interface IAdminRepository
    {
        List<User> GetAllUsers();
        string AddUser(User user);
        void UpdateUser(User user);
        void DisableUser(int userId);
        int GetSessionTimeout();
        bool IsAutoLogoutEnabled();
        SystemConfig GetSystemConfig();
        void UpdateSystemConfig(SystemConfig config);

        User GetUserById(int userId);
    }
}
