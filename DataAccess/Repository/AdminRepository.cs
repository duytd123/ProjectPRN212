using DataAccess.Models;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ProjectPrn212Context _context;

        private const string ConfigFilePath = "config.json";
        private readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "security_logs.txt");
        public AdminRepository(ProjectPrn212Context context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public string AddUser(User user)
        {
            bool isUserIdExist = _context.Users.Any(u => u.UserId == user.UserId);
            if (isUserIdExist)
            {
                return "Lỗi: UserId đã tồn tại!";
            }
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(user);
            bool isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            if (!isValid)
            {
                return string.Join("; ", validationResults.Select(v => v.ErrorMessage));
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            return "Thêm User thành công!";
        }


        public void UpdateUser(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser != null)
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
                existingUser.Role = user.Role;
                existingUser.Phone = user.Phone;
                existingUser.Address = user.Address;
                _context.SaveChanges();
            }
        }

        public void DisableUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.IsDisabled = true;
                _context.SaveChanges();
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
                    EnableAutoLogout = true
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
            return GetSystemConfig().SessionTimeout;
        }

        public bool IsAutoLogoutEnabled()
        {
            return GetSystemConfig().EnableAutoLogout;
        }


        public void EnableUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.IsDisabled = false;
                _context.SaveChanges();
            }
        }

    }
}

