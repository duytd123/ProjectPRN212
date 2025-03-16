using DataAccess.Repository.Interface;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
        private const string LogFilePath = "login_logs.txt";
        private static readonly object _lock = new();

        public class LoginLog
        {
            public DateTime Timestamp { get; set; }
            public int UserId { get; set; }
            public string Action { get; set; }
        }

        public User GetUserLogin(string email, string password)
        {
            try
            {
                using var context = new ProjectPrn212Context();
                var user = context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user != null)
                {
                    LogUserActivity(user.UserId, "logged in");
                }

                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public User GetUserByEmail(string email)
        {
            try
            {
                var context = new ProjectPrn212Context();
                return context.Users.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public User GetUserById(int id)
        {
            try
            {
                var context = new ProjectPrn212Context();
                return context.Users.FirstOrDefault(u => u.UserId == id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public void UpdateUser(User user)
        {
            try
            {
                var context = new ProjectPrn212Context();
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public void AddUser(User user)
        {
            try
            {
                var context = new ProjectPrn212Context();
                context.Add(user);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void LogUserActivity(int userId, string action)
        {
            string logEntry = $"{DateTime.Now}: User ID {userId} {action}.";
            lock (_lock)
            {
                File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
            }
        }

        public List<KeyValuePair<DateTime, Tuple<int, string>>> GetLoginLogs()
        {
            if (!File.Exists(LogFilePath))
                return new List<KeyValuePair<DateTime, Tuple<int, string>>>();

            var logs = File.ReadAllLines(LogFilePath)
                .Select(line =>
                {
                    var parts = line.Split(new[] { ": User ID " }, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        var timestampString = parts[0].Trim();
                        var actionString = parts[1].Trim();

                        if (DateTime.TryParse(timestampString, out DateTime timestamp))
                        {
                            var actionParts = actionString.Split(new[] { " " }, 2, StringSplitOptions.None);
                            if (actionParts.Length == 2 && int.TryParse(actionParts[0], out int userId))
                            {
                                return new KeyValuePair<DateTime, Tuple<int, string>>(timestamp, new Tuple<int, string>(userId, actionParts[1]));
                            }
                        }
                    }
                    return default(KeyValuePair<DateTime, Tuple<int, string>>);
                })
                .Where(log => log.Key != default(DateTime)) 
                .ToList();

            return logs;
        }

    }
}
