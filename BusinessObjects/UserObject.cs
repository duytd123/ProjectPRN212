using DataAccess.Repository.Interface;
using DataAccess.Models;
using DataAccess.Repository;

namespace BusinessObjects
{
    public class UserObject
    {
        private readonly IUserRepository _userRepository;

        public UserObject()
        {
            _userRepository = new UserRepository();
        }
        public User GetUserLogin(string email, string password)
        {
            var user = _userRepository.GetUserLogin(email, password);
            return user;
        }
        public User GetUserByEmail(string email) { return _userRepository.GetUserByEmail(email); }
        public void UpdateUser(User user) { _userRepository.UpdateUser(user); }
        public void AddUser(User user) { _userRepository.AddUser(user); }
        public void LogUserActivity(int userId, string action)
        {
            _userRepository.LogUserActivity(userId, action);
        }
        public List<KeyValuePair<DateTime, Tuple<int, string>>> GetLoginLogs()
        {
            return _userRepository.GetLoginLogs();
        }

    }
}
