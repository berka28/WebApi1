using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi1.Data;
using WebApi1.Models;

namespace WebApi1.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SqlDbContext _context;
        private IConfiguration _configuration { get; }


        public IdentityService(SqlDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

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
                        if (user.ValidatePasswordHash(password))
                        {
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var _secretKey = Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value);

                            var tokenDescriptor = new SecurityTokenDescriptor
                            {
                                Subject = new ClaimsIdentity(new Claim[] { new Claim("UserID", user.Id.ToString())}),
                                Expires = DateTime.Now.AddHours(1),
                                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretKey), SecurityAlgorithms.HmacSha512Signature)
                            };

                            var _accesstoken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

                            _context.SessionTokens.Add(new SessionToken { UserId = user.Id, AccessToken = _accesstoken });
                            await _context.SaveChangesAsync();

                            return new LogInResponseModel 
                            { 
                                Success = true, 
                                Result = new LogInResponseResult 
                                {
                                    Id = user.Id,
                                    Email = user.Email,
                                    AccessToken = _accesstoken
                                }
                            };
                        }
                        
                    }
                    catch { }
                }
            }
            catch { }

            return new LogInResponseModel { Success = false};

        }

        public async Task<IEnumerable<UserResponse>> GetUsers(RequestUser requestUser)
        {
            var users = new List<UserResponse>();

            if (_context.SessionTokens.Any(x => x.UserId == requestUser.UserId && x.AccessToken == requestUser.AccessToken))
            {
                foreach(var user in await _context.Users.ToListAsync())
                {
                    users.Add(new UserResponse { FirstName = user.FirstName, LastName = user.Lastname, Email = user.Email });
                }
            }
            return users;
        }
    }
}
