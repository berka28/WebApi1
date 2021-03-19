using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi1.Data;
using WebApi1.Models;

namespace WebApi1.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SqlDbContext _context;

        public IdentityService(SqlDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateUserAsync(RegisterModel model)
        {
            if (!_context.Users.Any(user => user.Email == model.Email))
            {
                try
                {
                    var user = new User()
                    {
                        FirstName = model.FirstName,
                        Lastname = model.LastName,
                        Email = model.Email
                    };
                    user.CreatePasswordWithHash(model.Password);
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    return true;
                }
                catch {}
            }

            return false;
        }

        public async Task<LogInResponseModel> LogInAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);

                if (_context.Users.Any(user => user.Email == email))
                {
                    try
                    {
                        return new LogInResponseModel { Success = true };
                    }
                    catch { }
                }
            }
            catch { }

            return new LogInResponseModel { Success = false};

        }
    }
}
