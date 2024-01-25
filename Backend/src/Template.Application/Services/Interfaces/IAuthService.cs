using Template.Application.Models.Results;
using Template.Application.Requests.Auth;
using Template.Application.Responses.Auth;

namespace Template.Application.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        public Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request);
    }
}
