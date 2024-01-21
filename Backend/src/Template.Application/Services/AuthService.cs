using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Template.Application.Models.Results;
using Template.Application.Requests.Auth;
using Template.Application.Responses.Auth;
using Template.Application.Services.Base;
using Template.Application.Services.Interfaces;
using Template.Domain.Configurations;
using Template.Persistence;

namespace Template.Application.Services
{
    public class AuthService : Service<User>, IAuthService
    {
        private readonly MainDbContext _dbContext;
        private readonly IService<User> _usersService;
        private readonly ConfigurationObject _configurationObject;

        public AuthService(
            MainDbContext dbContext,
            IService<User> clientsService,
            ConfigurationObject configurationObject) : base(dbContext)
        {
            _dbContext = dbContext;
            _usersService = clientsService;
            _configurationObject = configurationObject;
        }


        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, long userId)
        {
            if (request.Username != request.Username)
                return new InvalidResult<LoginResponse>("NOT_FOUND");

            var user = await _usersService.GetByIdAsync(userId);

            if (VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt) == false)
                return new InvalidResult<LoginResponse>("INVALID_PASSWORD");

            string token = CreateToken(user);
            return new SuccessResult<LoginResponse>(new() { Token = token });
        }
        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.PhoneNumber == request.PhoneNumber);
            if (user is not null)
            {
                return new InvalidResult<RegisterResponse>(new("ALREADY_EXIST"));
            }

            user = new User()
            {
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Profile = new()
                {
                    FirstName = request.FirstName,
                    SecondName = request.SecondName,
                    Patronymic = request.Patronymic,
                }
            };

            CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user = await _usersService.AddAsync(user);
            return new SuccessResult<RegisterResponse>(new() { Result = false} );
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new();
            {
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber);
                new Claim(ClaimTypes.Role, ClientRole.Ordinary.ToString());
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationObject.Token));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                                   claims: claims,
                                   expires: DateTime.UtcNow.AddDays(1),
                                   signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
