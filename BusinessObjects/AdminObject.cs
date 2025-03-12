using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BusinessObjects
{

    public class AdminObject
    {
        private readonly IAdminRepository _adminRepository;

        private const string ConfigFilePath = "config.json";
        public AdminObject(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public List<User> GetAllUsers()
        {
            return _adminRepository.GetAllUsers();
        }

        public void AddUser(User user)
        {
            if (user != null)
            {
                _adminRepository.AddUser(user);
            }
        }

        public void UpdateUser(User user)
        {
            if (user != null)
            {
                _adminRepository.UpdateUser(user);
            }
        }

        public void DisableUser(int userId)
        {
            var user = _adminRepository.GetAllUsers().FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                _adminRepository.DisableUser(userId);
            }
        }


        public SystemConfig GetSystemConfig()
        {
            if (!File.Exists(ConfigFilePath))
            {
                var defaultConfig = new SystemConfig
                {
                    SessionTimeout = 30,
                    EnableLogging = false,
                    EnableTwoFactorAuth = false,
                    EnableAutoLogout = false
                };

                string defaultJson = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                File.WriteAllText(ConfigFilePath, defaultJson);
                return defaultConfig;
            }

            string json = File.ReadAllText(ConfigFilePath);
            return JsonConvert.DeserializeObject<SystemConfig>(json);
        }

        public void UpdateSystemConfig(SystemConfig config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }
        public int GetSessionTimeout()
        {
            return _adminRepository.GetSessionTimeout();
        }

        public bool IsAutoLogoutEnabled()
        {
            return _adminRepository.IsAutoLogoutEnabled();
        }

        public void EnableUser(int userId)
        {
            var user = _adminRepository.GetUserById(userId);
            if (user != null)
            {
                user.IsDisabled = false;
                _adminRepository.UpdateUser(user);
            }
        }

    }
}
