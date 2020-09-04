using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS_Auth_Service.Models;

namespace VMS_Auth_Service.Repository
{
    public interface IAuthRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        //Task<bool> UpdateUser(string uid, User user);
        Task<User> SaveUser(User user);
        bool UserExists(string email, string type);
        Task<User> Register(User user, string password);
        Task<User> Login(string email, string password, string type);
    }
}
