using DataAccess.Models;

namespace DataAccess.Repository.Interface
{
    public interface IUserRepository
    {
        public User GetUserLogin(string email, string password);
        public User GetUserByEmail(string email);
        public void UpdateUser(User user);
        public void AddUser(User user);
    }
}
