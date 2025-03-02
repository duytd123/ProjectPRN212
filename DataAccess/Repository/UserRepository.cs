using DataAccess.Repository.Interface;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
        public User GetUserLogin(string email, string password)
        {
            try
            {
                var context = new ProjectPrn212Context();
                return context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
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
    }
}
