using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Models.Results;
using Template.Application.Requests.Auth;
using Template.Application.Responses.Auth;
using Template.Application.Services.Interfaces;

namespace Template.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login"), AllowAnonymous]
        public async Task<Result<LoginResponse>> Login(LoginRequest request)
        {
            if (request is not null)
            {
                var result = await _authService.LoginAsync(request);
                return result;
            }
            throw new("INVALID_REQUEST");
        }

        [HttpPost]
        [Route("register"), AllowAnonymous]
        public async Task<Result<RegisterResponse>> Register(RegisterRequest request)
        {
            if (request is not null)
            {
                var result = await _authService.RegisterAsync(request);
                return result;
            }
            throw new("INVALID_REQUEST");
        }

        [HttpGet]
        [Route("check"), Authorize]
        public async Task<Result<bool>> Check()
        { 
            return new SuccessResult<bool>(true);
        }
    }
}
