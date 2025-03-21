﻿using DataAccess.Models;

namespace DataAccess.Repository.Interface
{
    public interface IUserRepository
    {
        public User GetUserLogin(string email, string password);
        public User GetUserByEmail(string email);
        public User GetUserById(int id);
        public void UpdateUser(User user);
        public void AddUser(User user);
        void LogUserActivity(int userId, string action);
        List<KeyValuePair<DateTime, Tuple<int, string>>> GetLoginLogs();
    }
}
