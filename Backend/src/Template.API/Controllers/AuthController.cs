using Template.Application.Models.Results;
using Template.Application.Requests.Auth;
using Template.Application.Responses.Auth;
using Template.Application.Services.Audit;
using Template.Application.Services.Interfaces;
using Template.Persistence;

namespace Template.API.Controllers
{
    public class AuthController
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
         
        public async Task<Result<LoginResponse>> Login(LoginRequest request)
        {
            var userId = _auditUserProvider.GetUserId().Value;
            var result = await _authService.LoginAsync(request, userId);
            return result;
        }

        public async Task<Result<RegisterResponse>> Login(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return result;
        }

    }
}
