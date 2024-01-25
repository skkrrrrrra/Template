using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Template.Application.Common.Helpers;
using Template.Application.Models.Results;
using Template.Application.Requests.Auth;
using Template.Application.Responses.Auth;
using Template.Application.Services.Base;
using Template.Application.Services.Interfaces;
using Template.Domain.Configurations;
using Template.Persistence.Context;

namespace Template.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly MainDbContext _dbContext;
        private readonly Service<User> _usersService;
        private readonly ConfigurationObject _configurationObject;

        public AuthService(
            MainDbContext dbContext,
            Service<User> usersService,
            ConfigurationObject configurationObject)
        {
            _dbContext = dbContext;
            _usersService = usersService;
            _configurationObject = configurationObject;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) == true || string.IsNullOrEmpty(request.Password) == true)
            {
                throw new("INVALID_REQUEST");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == request.Username);

            if(user is null)
            {
                throw new("USER_NOT_FOUND");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new ("INVALID_PASSWORD");
            }
            string token = CreateToken(user);
            return new SuccessResult<LoginResponse>(new() { Token = token });
        }
        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var requestVerifyResult = VerifyRegisterRequest(request);
            var passwordVerifyResult = VerifyPassword(request.Password);
            if(passwordVerifyResult.Count > 0 || requestVerifyResult.Count > 0)
            {
                var errors = requestVerifyResult.Concat(passwordVerifyResult).ToList();
                return new InvalidResult<RegisterResponse>(errors);
            }

            User user;
            try
            {
                user = await _dbContext.Users
                    .FirstOrDefaultAsync(item => 
                        item.PhoneNumber == request.PhoneNumber || item.Username == request.Username);

                if (user is not null)
                {
                    return new InvalidResult<RegisterResponse>("ALREADY_EXIST");
                }
            }
            catch(Exception ex)
            {
                return new InvalidResult<RegisterResponse>(ex.Message);
            }

            CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);
            user = new User()
            {
                Username = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Profile = new()
                {
                    FirstName = request.FirstName,
                    SecondName = request.SecondName,
                    Patronymic = request.Patronymic,
                }
            };

            user = await _usersService.AddAsync(user);
            return new SuccessResult<RegisterResponse>(new() { Result = true } );
        }

        private string CreateToken(User user)
        {
            var jwt = string.Empty;
            try
            {
                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, ClientRole.Ordinary.ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationObject.Jwt.Key));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var token = new JwtSecurityToken(
                                       claims: claims,
                                       expires: DateHelper.GetCurrentDateTime().AddDays(1),
                                       signingCredentials: cred);

                jwt = new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw;
            }
            return jwt;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private List<string> VerifyPassword(string password)
        {
            var result = new List<string>();

            var containsMoreThanEightSymbols = new Regex("^.{8,20}$");
            if (string.IsNullOrEmpty(password) || containsMoreThanEightSymbols.Match(password).Success == false)
            {
                result.Add("Длина пароля должна быть больше 8 и меньше 20 символов");
                return result;
            }

            var containsOneCapitalAndLowercaseEnglishLetter = new Regex(".*[a-zA-Z].*");
            if (containsOneCapitalAndLowercaseEnglishLetter.Match(password).Success == false)
            {
                result.Add("В пароле должна быть хотя бы одна заглавная и одна строчная английские буквы");
            }

            var containsOneNumeric = new Regex("(?=.*?[0-9])");
            if (containsOneNumeric.Match(password).Success == false)
            {
                result.Add("В пароле должна быть хотя бы одна цифра");
            }

            var containsOneSpecialSymbol = new Regex("(?=.*?[#?!@$%^&*-])");
            if(containsOneSpecialSymbol.Match(password).Success == false)
            {
                result.Add("Пароль должен содержать хотя бы один символ");
            }

            return result;
        }
        private List<string> VerifyRegisterRequest(RegisterRequest request)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                result.Add("Поле 'Номер телефона' не может быть пустым");
            }
            if (string.IsNullOrEmpty(request.Username))
            {
                result.Add("Поле 'Логин' не может быть пустым");
            }
            if (string.IsNullOrEmpty(request.Email))
            {
                result.Add("Поле 'Электронная почта' не может быть пустым");
            }
            if (string.IsNullOrEmpty(request.FirstName))
            {
                result.Add("Поле 'Имя' не может быть пустым");
            }
            if (string.IsNullOrEmpty(request.SecondName))
            {
                result.Add("Поле 'Фамилия' не может быть пустым");
            }
            if (string.IsNullOrEmpty(request.Patronymic))
            {
                result.Add("Поле 'Фамилия' не может быть пустым");
            }
            return result;
        }
    }
}
