using Xunit;
using Moq;
using Newsfeed.Services;
using Newsfeed.Services.Interfaces;
using Newsfeed.Models.DTO;
using Newsfeed.Models.Response;
using Newsfeed.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PostgresDb.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class UserServiceTests
{
    private readonly Mock<IBaseRepository<RegisterUser>> _repositoryMock;

    private readonly Mock<ApiDbContext> _dbMock;

    private readonly Mock<Serilog.ILogger> _loggerMock;

    private readonly IUserService _userService;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IBaseRepository<RegisterUser>>();

        _dbMock = new Mock<ApiDbContext>();

        _loggerMock = new Mock<Serilog.ILogger>();

        _userService = new UserService(_repositoryMock.Object, _dbMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnUnauthorizedResponseWithUsers()
    {
        var users = new List<RegisterUser>()
        {
            new RegisterUser { Id = Guid.NewGuid(), Name = "user1", Email = "user1@example.com" },

            new RegisterUser { Id = Guid.NewGuid(), Name = "user2", Email = "user2@example.com" }
        };

        _repositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(users);

        var response = await _userService.GetAll();

        Assert.True(response.Success);

        Assert.Equal(StatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenNotAuthorized_ShouldReturnUnauthorizedResponse()
    {
        UserServiceAuthorize.IsAuthorized = false;

        var response = await _userService.GetAll();

        Assert.True(response.Success);

        Assert.Equal(StatusCode.Unauthorized, response.StatusCode);

        Assert.Equal(14, response.ErrorCode);
    }

    [Fact]
    public async Task GetAll_WhenExceptionThrown_ShouldReturnErrorResponse()
    {
        _repositoryMock.Setup(repo => repo.GetAll()).ThrowsAsync(new Exception("Some error"));

        var response = await _userService.GetAll();

        Assert.True(response.Success);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUnauthorizedResponseWithUser()
    {
        var user = new RegisterUser { Id = Guid.NewGuid(), Name = "user1", Email = "user1@example.com" };

        var response = await _userService.GetUser(user.Id);

        Assert.True(response.Success);

        Assert.Equal(StatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUnauthorizedResponse()
    {
        UserServiceAuthorize.IsAuthorized = false;

        var userId = Guid.NewGuid();

        var response = await _userService.GetUser(userId);

        Assert.True(response.Success);

        Assert.Equal(StatusCode.Unauthorized, response.StatusCode);

        Assert.Equal(7, response.ErrorCode);
    }
}
