using Microsoft.EntityFrameworkCore;
using SurpassIntegration.Data;
using SurpassIntegration.Models;

namespace SurpassIntegration.Services
{
    public interface IUserService
    {
        User? ValidateUser(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User? ValidateUser(string username, string password)
        {
            // In production, compare hashed passwords
            return _dbContext.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }
}
