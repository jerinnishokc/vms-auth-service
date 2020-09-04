using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS_Auth_Service.Models;

namespace VMS_Auth_Service.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly woodsContext _context;
        private readonly IConfiguration _config;

        public AuthRepository(woodsContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: api/Users
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        public async Task<User> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return null;//NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //public async Task<bool> UpdateUser(string email, User user)
        //{
        //    if (uid != user.Uid)
        //    {
        //        return false;//BadRequest();
        //    }

        //    _context.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserExists(uid))
        //        {
        //            return false;// NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return false;//NoContent();
        //}

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        
        public async Task<User> SaveUser(User user)
        {
            _context.User.Add(user);
            var response = await _context.SaveChangesAsync();

            return user;
        }

        // DELETE: api/Users/5
        //public async Task<ActionResult<User>> DeleteUser(int id)
        //{
        //    var user = await _context.User.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.User.Remove(user);
        //    await _context.SaveChangesAsync();

        //    return user;
        //}

        public bool UserExists(string email, string type)
        {
            return _context.User.Any(e => e.Email == email && e.Type == type);
        }
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.User.Add(user);
            var response = await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<User> Login(string email, string password, string type)
        {

            var user = await _context.User.FindAsync(email,type);
                       
            //var user = await _context.User.FindAsync(email);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }
    }
}
