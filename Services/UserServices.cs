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

namespace Newsfeed.Services;

public class UserService : IUserService
{
    private readonly IBaseRepository<RegisterUser> _repository;

    private readonly ApiDbContext _db;

    private readonly Serilog.ILogger _logger;

    public UserService(IBaseRepository<RegisterUser> repository, ApiDbContext db, Serilog.ILogger logger)
    {
        _repository = repository;

        _db = db;

        _logger = logger;
    }

    public async Task<BaseResponse<RegisterUser>> GetAll()
    {
        try
        {
            if (!UserServiceAuthorize.IsAuthorized)
            {
                _logger.Error("Error getting user information. The user is not authorized.");

                return new BaseResponse<RegisterUser>()
                {
                    Success = true,

                    StatusCode = StatusCode.Unauthorized,

                    ErrorCode = 14
                };
            }

            var users = await _repository.GetAll();

            _logger.Information("Successfully retrieved user information");

            return new BaseResponse<RegisterUser>()
            {
                Success = true,

                StatusCode = StatusCode.OK,

                statusCode = 1,

                Data = new ObjectResponse<RegisterUser>()
                {
                    Content = users
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error("Error getting information about users: " + ex.Message);

            return new BaseResponse<RegisterUser>()
            {
                Success = true,

                StatusCode = StatusCode.BadRequest,

                TimeStamp = DateTime.Now,
            };
        }
    }

    public async Task<BaseResponse<RegisterUser>> GetUser(Guid id)
    {
        try
        {
            if (!UserServiceAuthorize.IsAuthorized)
            {
                _logger.Error("Error getting information about the user, because he was not authorized.");

                return new BaseResponse<RegisterUser>()
                {
                    Success = true,

                    StatusCode = StatusCode.Unauthorized,

                    ErrorCode = 7
                };
            }
            else if (id == null)
            {
                _logger.Error("Failed to get user information because the id parameter is null.");

                return new BaseResponse<RegisterUser>()
                {
                    Success = true,

                    StatusCode = StatusCode.BadRequest,

                    ErrorCode = 14
                };
            }

            var users = await _db.V1_create_users_table.SingleOrDefaultAsync(x => x.Id == id);

            var user = new RegisterUser()
            {
                Id = users.Id,

                Avatar = users.Avatar,

                Email = users.Email,

                Name = users.Name,

                Role = users.Role,

                Password = users.Password
            };

            _logger.Information($"User information {user.Name} with id {id} sent successfully.");

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
            _logger.Error($"Error getting user information: {ex}");

            return new BaseResponse<RegisterUser>()
            {
                Success = true,

                StatusCode = StatusCode.BadRequest,

                TimeStamp = DateTime.Now,
            };
        }
    }

    public async Task<BaseResponse<RegisterUser>> GetUserInfo()
    {
        try
        {
            if (!UserServiceAuthorize.IsAuthorized)
            {
                _logger.Error("Error getting information about the user, because he was not authorized.");

                return new BaseResponse<RegisterUser>()
                {
                    Success = true,

                    StatusCode = StatusCode.Unauthorized,

                    ErrorCode = 7
                };
            }

            var users = await _db.V1_create_users_table.SingleOrDefaultAsync(x => x.Id == UserServiceAuthorize.userId);

            var user = new RegisterUser()
            {
                Id = users.Id,

                Avatar = users.Avatar,

                Email = users.Email,

                Name = users.Name,

                Role = users.Role,

                Password = users.Password,

                Token = users.Token
            };

            _logger.Information($"User information {user.Name} with id {UserServiceAuthorize.userId} sent successfully.");

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
            _logger.Error($"Error getting user information: {ex}");

            return new BaseResponse<RegisterUser>()
            {
                Success = true,

                StatusCode = StatusCode.BadRequest,

                TimeStamp = DateTime.Now,
            };
        }
    }

    public async Task<BaseResponse<RegisterUser>> ChangeData(PutUserDTO user)
    {
        try
        {
            if (!UserServiceAuthorize.IsAuthorized)
            {
                _logger.Error("Error getting user information. The user is not authorized.");

                return new BaseResponse<RegisterUser>()
                {
                    Success = true,

                    StatusCode = StatusCode.Unauthorized,

                    ErrorCode = 14
                };
            }

            var users = await _db.V1_create_users_table.ToListAsync();

            var Update = UserServiceAuthorize.userId;

            var userToUpdate = await _db.V1_create_users_table.FindAsync(Update);

            if (userToUpdate == null)
            {
                _logger.Error("Error updating user data because it was not found in the database.");

                return new BaseResponse<RegisterUser>()
                {
                    Success = false,

                    StatusCode = StatusCode.NotFound,
                };
            }

            if (user.Email == "string" && user.Name == "string")
            {
                userToUpdate.Email = userToUpdate.Email;

                userToUpdate.Name = userToUpdate.Name;

                userToUpdate.Role = user.Role;

                userToUpdate.Avatar = user.Avatar;
            }
            else if (user.Email == "string")
            {
                userToUpdate.Email = userToUpdate.Email;

                userToUpdate.Name = user.Name;

                userToUpdate.Role = user.Role;

                userToUpdate.Avatar = user.Avatar;
            }
            else
            {
                userToUpdate.Avatar = user.Avatar;

                userToUpdate.Email = user.Email;

                userToUpdate.Name = user.Name;

                userToUpdate.Role = user.Role;
            }

            await _repository.Save();

            var updatedUser = new RegisterUser()
            {
                Id = userToUpdate.Id,

                Avatar = userToUpdate.Avatar,

                Email = userToUpdate.Email,

                Name = userToUpdate.Name,

                Role = userToUpdate.Role,

                Password = userToUpdate.Password,

                Token = userToUpdate.Token

            };

            _logger.Information($"User data {user.Name} with Email: {user.Email} successfully updated.");

            return new BaseResponse<RegisterUser>()
            {
                Success = true,

                StatusCode = StatusCode.OK,

                statusCode = 1,

                Data = new ObjectResponse<RegisterUser>()
                {
                    Content = updatedUser
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error($"An error occurred while updating data: {ex}");

            return new BaseResponse<RegisterUser>()
            {
                Success = false,
                StatusCode = StatusCode.BadRequest,
            };
        }
    }

    public async Task<BaseResponse<RegisterUser>> DeleteUser()
    {
        try
        {
            if (!UserServiceAuthorize.IsAuthorized)
            {
                _logger.Error($"Error deleting users because the user is not authorized");

                return new BaseResponse<RegisterUser>()
                {
                    Success = true,

                    StatusCode = StatusCode.Unauthorized,

                    ErrorCode = 7
                };
            }

            var selectedUser = await _db.V1_create_users_table.FindAsync(UserServiceAuthorize.userId);

            _repository.Delete(selectedUser);


            _logger.Information($"All user data deleted by user: {UserServiceAuthorize.userName} with id: {UserServiceAuthorize.userId} and Email: {UserServiceAuthorize.userEmail}");

            UserServiceAuthorize.IsAuthorized = false;

            return new BaseResponse<RegisterUser>()
            {
                StatusCode = StatusCode.OK,

                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.Error($"An error occurred while deleting data: {ex}");

            return new BaseResponse<RegisterUser>()
            {
                Success = true,

                StatusCode = StatusCode.BadRequest,

                TimeStamp = DateTime.Now,
            };
        }
    }
}
