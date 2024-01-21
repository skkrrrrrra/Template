using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Models.Results;
using Template.Application.Requests.Auth;
using Template.Application.Responses.Auth;
using Template.Application.Services.Interfaces;
using Template.Persistence;

namespace Template.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService; 
        private IAuditUserProvider _auditUserProvider;

        public AuthController(
            IAuthService authService,
            IAuditUserProvider auditUserProvider)
        {
            _auditUserProvider = auditUserProvider;
            _authService = authService;
        }

        [HttpPost]
        [Route("login"), AllowAnonymous]
        public async Task<Result<LoginResponse>> Login(LoginRequest request)
        {
            var userId = _auditUserProvider.GetUserId().Value;
            var result = await _authService.LoginAsync(request, userId);
            return result;
        }

        [HttpPost]
        [Route("register"), AllowAnonymous]
        public async Task<Result<RegisterResponse>> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return result;
        }

    }
}
