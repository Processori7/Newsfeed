using Microsoft.EntityFrameworkCore;
using PostgresDb.Data;
using Newsfeed.Services.Interfaces;
using Newsfeed.Models.DTO;
using Newsfeed.Models.Enums;
using Newsfeed.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using DotNetEnv;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Newsfeed.Services
{
    public class UserServiceAuthorize : IUserServiceAuthorize
    {
        public static bool IsAuthorized = false;

        public static string userEmail;

        public static Guid userId;

        public static string userName;

        private readonly IBaseRepository<RegisterUser> _repository;

        private readonly ApiDbContext _db;

        private readonly Serilog.ILogger _logger;

        public UserServiceAuthorize(IBaseRepository<RegisterUser> repository, ApiDbContext dbContext, Serilog.ILogger logger)
        {
            _repository = repository;

            _db = dbContext;

            _logger = logger;
        }

        public static RegisterUser user = new RegisterUser();

        public async Task<BaseResponse<RegisterUser>> Register(RegisterUserDTO registerUserDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(registerUserDTO.Email)
                || string.IsNullOrWhiteSpace(registerUserDTO.Password)
                || string.IsNullOrWhiteSpace(registerUserDTO.Name)
                || string.IsNullOrWhiteSpace(registerUserDTO.Role))
                {
                    _logger.Error("An error occurred while registering the user: " + registerUserDTO.Email);

                    return await Task.FromResult(new BaseResponse<RegisterUser>()
                    {
                        StatusCode = StatusCode.BadRequest
                    });
                }

                string pattern = @"^.+@.+$";
                bool isValidEmail = Regex.IsMatch(registerUserDTO.Email, pattern);

                if (!isValidEmail)
                {
                    _logger.Error("An error occurred while registering the user: " + registerUserDTO.Email + " because mail validation failed");

                    return new BaseResponse<RegisterUser>()
                    {
                        Success = false,

                        statusCode = 17,

                        StatusCode = StatusCode.BadRequest,

                        TimeStamp = DateTime.Now
                    };
                }

                if (_db.V1_create_users_table.Any(x => x.Email == registerUserDTO.Email))
                {
                    return new BaseResponse<RegisterUser>()
                    {
                        Success = false,

                        StatusCode = StatusCode.BadRequest,

                        TimeStamp = DateTime.Now,
                    };
                }

                var user3 = new RegisterUser()
                {
                    Id = registerUserDTO.Id,

                    Name = registerUserDTO.Name,

                    Role = registerUserDTO.Role,

                    Email = registerUserDTO.Email,

                    Password = BCrypt.Net.BCrypt.HashPassword(registerUserDTO.Password),

                    Avatar = registerUserDTO.Avatar,

                    Token = null
                };

                string token_gen = GenerateJwtToken(user3);

                var user = new RegisterUser()
                {
                    Id = registerUserDTO.Id,

                    Name = registerUserDTO.Name,

                    Role = registerUserDTO.Role,

                    Email = registerUserDTO.Email,

                    Password = BCrypt.Net.BCrypt.HashPassword(registerUserDTO.Password),

                    Avatar = registerUserDTO.Avatar,

                    Token = token_gen
                };

                await _repository.Create(user);

                _logger.Information("User registered using email:" + registerUserDTO.Email);

                return new BaseResponse<RegisterUser>()
                {
                    Success = true,

                    StatusCode = StatusCode.OK,

                    statusCode = 1,

                    Data = new ObjectResponse<RegisterUser>()
                    {
                        Content = user
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while registering the user: " + ex.Message);

                return new BaseResponse<RegisterUser>()
                {
                    Success = true,

                    StatusCode = StatusCode.BadRequest,

                    TimeStamp = DateTime.Now,
                };
            }
        }

        public async Task<BaseResponse<RegisterUser>> Login(AuthUserDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || request.Email == "string")
                {
                    return new BaseResponse<RegisterUser>()
                    {
                        Success = false,

                        StatusCode = StatusCode.BadRequest,

                        ErrorCode = 14
                    };
                }

                var user = await _db.V1_create_users_table.FirstOrDefaultAsync(x => x.Email == request.Email);

                if (user == null)
                {
                    _logger.Error("An error occurred while authorizing the user, because the user enters incorrect data for authorization");
                    return new BaseResponse<RegisterUser>()
                    {
                        Success = false,

                        StatusCode = StatusCode.BadRequest,

                        ErrorCode = 14
                    };
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    return new BaseResponse<RegisterUser>()
                    {
                        Success = false,

                        StatusCode = StatusCode.Unauthorized,

                        ErrorCode = 14
                    };
                }
                else
                {

                    var user2 = new RegisterUser()
                    {
                        Id = user.Id,

                        Name = user.Name,

                        Role = user.Role,

                        Email = user.Email,

                        Password = user.Password,

                        Avatar = user.Avatar,

                        Token = user.Token
                    };

                    IsAuthorized = true;

                    userEmail = user2.Email;

                    userId = user2.Id;

                    userName = user2.Name;

                    _logger.Information("User logged in using email: " + userEmail + " and name: " + userName);

                    return new BaseResponse<RegisterUser>()
                    {
                        Success = true,

                        StatusCode = StatusCode.OK,

                        statusCode = 1,

                        Data = new ObjectResponse<RegisterUser>()
                        {
                            Content = user2
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while authorizing the user: " + ex.Message);

                return new BaseResponse<RegisterUser>()
                {
                    Success = true,

                    StatusCode = StatusCode.BadRequest,

                    TimeStamp = DateTime.Now,
                };
            }
        }
        
        public string GenerateJwtToken(RegisterUser user)
        {
            try
            {

                List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),

                new Claim(ClaimTypes.Name, user.Name),

                new Claim(ClaimTypes.Actor, user.Avatar),

                new Claim(ClaimTypes.Role, user.Role)
            };

                var token = Environment.GetEnvironmentVariable("Token");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var genToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(genToken);

                _logger.Information("JWT token successfully generated");

                return jwt;
            }

            catch (Exception ex)
            {
                _logger.Error("An error occurred while generating the token:" + ex.Message);

                return $" {StatusCode.BadRequest}\nBad Request";
            }
        }
    }
}
