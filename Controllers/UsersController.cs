using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi1.Data;
using WebApi1.Models;
using WebApi1.Services;

namespace WebApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IIdentityService _identity;

        public UsersController(IIdentityService identity)
        {
            _identity = identity;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (await _identity.CreateUserAsync(model))
                return new OkResult();

            return new BadRequestResult();
        }

        [AllowAnonymous]
        [HttpPost("login")]

        public async Task<IActionResult> LogIn([FromBody] LogInModel model)
        {
            var response = await _identity.LogInAsync(model.Email, model.Password);

            if (response.Success)
                return new OkObjectResult(response.Result);

            return new BadRequestResult();
        }

        [HttpGet]
        public async Task<IEnumerable<UserResponse>> GetUsers()
        {
            return await _identity.GetUsersAsync(IdentityRequestData());
        }

        private RequestUser IdentityRequestData()
        {
            HttpContext.Request.Headers.TryGetValue("UserId", out var _userId);
            HttpContext.Request.Headers.TryGetValue("AccessToken", out var _accessToken);

            return new RequestUser
            {
                UserId = int.Parse(_userId),
                AccessToken = _accessToken
            };
        }
    }
}
