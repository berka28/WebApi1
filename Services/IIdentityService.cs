using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi1.Models;

namespace WebApi1.Services
{
    public interface IIdentityService
    {
        Task<bool> CreateUserAsync(RegisterModel model);

        Task<LogInResponseModel> LogInAsync(string email, string password);
    }
}
