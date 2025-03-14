using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BusinessObjects
{
    public static class SessionManager
    {
        private const string LogFilePath = "login_logs.txt";
        private static readonly object _lock = new();

        public static void UserLoggedIn(int userId, string username, string email)
        {
            string logEntry = $"{DateTime.Now}: {username} (ID: {userId}) logged in.";
            WriteToFile(logEntry);
        }
        public static void UserLoggedOut(int userId, string username)
        {
            string logEntry = $"{DateTime.Now}: {username} (ID: {userId}) logged out.";
            WriteToFile(logEntry);
        }

        private static void WriteToFile(string logEntry)
        {
            lock (_lock)
            {
                File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
            }
        }

        public static List<string> GetLoginLogs()
        {
            return File.Exists(LogFilePath) ? new List<string>(File.ReadAllLines(LogFilePath)) : new List<string> { "No login history found." };
        }
    }
}

