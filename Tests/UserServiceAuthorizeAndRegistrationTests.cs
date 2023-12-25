using Moq;
using Newsfeed.Models.DTO;
using Newsfeed.Models.Enums;
using Newsfeed.Models.Response;
using Newsfeed.Services;
using PostgresDb.Data;


namespace Newsfeed.Tests.Services
{
    public class UserServiceAuthorizeAndRegistrationTests
    {
        private readonly Mock<IBaseRepository<RegisterUser>> _mockRepository;

        private readonly Mock<ApiDbContext> _mockDbContext;

        private readonly Mock<Serilog.ILogger> _mockLogger;

        private readonly UserServiceAuthorize _service;

        private readonly UserService _userService;

        public UserServiceAuthorizeAndRegistrationTests()
        {
            _mockRepository = new Mock<IBaseRepository<RegisterUser>>();

            _mockDbContext = new Mock<ApiDbContext>();

            _mockLogger = new Mock<Serilog.ILogger>();

            _service = new UserServiceAuthorize(_mockRepository.Object, _mockDbContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsSuccessResponse()
        {
            var registerUserDto = new RegisterUserDTO
            {
                Email = "test@example.com",

                Password = "password",

                Name = "John",

                Role = "User"
            };
            var baseResponse = new BaseResponse<RegisterUser>
            {
                Success = true,

                StatusCode = StatusCode.OK,

                statusCode = 1,

                Data = new ObjectResponse<RegisterUser>
                {
                    Content = new RegisterUser
                    {
                        Email = registerUserDto.Email,

                        Name = registerUserDto.Name,

                        Role = registerUserDto.Role
                    }
                }
            };
        }

        [Fact]
        public async Task Register_WithNullOrWhitespaceData_ReturnsBadRequestResponse()
        {
            var registerUserDto = new RegisterUserDTO
            {
                Email = null,

                Password = "  ",

                Name = "",

                Role = "Admin"
            };

            var result = await _service.Register(registerUserDto);

            Assert.False(result.Success);

            Assert.Equal(StatusCode.BadRequest, result.StatusCode);
        }


        [Fact]
        public async Task Register_WithInvalidEmail_ReturnsBadRequestResponse()
        {
            var registerUserDto = new RegisterUserDTO
            {
                Email = "invalid_email",

                Password = "password",

                Name = "John",

                Role = "User"
            };

            var result = await _service.Register(registerUserDto);

            Assert.False(result.Success);
            
            Assert.Equal(StatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task RegisterWithExistingEmailReturnsBadRequestResponse()
        {
            var registerUserDto = new RegisterUserDTO
            {
                Email = "johny@test",

                Password = "string",

                Name = "johny",

                Role = "user"
            };

            var result = await _service.Register(registerUserDto);

            Assert.True(result.Success);

            Assert.Equal(StatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsSuccessResponse()
        {
            var authUserDto = new AuthUserDTO
            {
                Email = "Johny@Test",

                Password = "string"
            };
            var registerUser = new RegisterUser
            {
                Id = Guid.NewGuid(),

                Email = authUserDto.Email,

                Password = BCrypt.Net.BCrypt.HashPassword(authUserDto.Password),

                Name = "Johny",

                Role = "User"
            };
            var baseResponse = new BaseResponse<RegisterUser>
            {
                Success = true,

                StatusCode = StatusCode.OK,

                statusCode = 1,

                Data = new ObjectResponse<RegisterUser>
                {
                    Content = new RegisterUser
                    {
                        Email = registerUser.Email,

                        Name = registerUser.Name,

                        Role = registerUser.Role
                    }
                }
            };

            var result = await _service.Login(authUserDto);

            var user = new RegisterUser();

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Login_WithNullOrWhitespaceEmail_ReturnsBadRequestResponse()
        {
            var authUserDto = new AuthUserDTO
            {
                Email = null,

                Password = "password"
            };

            var result = await _service.Login(authUserDto);

            Assert.False(result.Success);

            Assert.Equal(StatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Login_WithNonExistingEmail_ReturnsBadRequestResponse()
        {
            var authUserDto = new AuthUserDTO
            {
                Email = "nonexisting_email@example.com",

                Password = "password"
            };

            var result = await _service.Login(authUserDto);

            Assert.True(result.Success);

            Assert.Equal(StatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsBadRequestResponse()
        {
            var authUserDto = new AuthUserDTO
            {
                Email = "Johny@Test",

                Password = "invalid_password"
            };

            var registerUser = new RegisterUser
            {
                Id = Guid.NewGuid(),

                Email = authUserDto.Email,

                Password = BCrypt.Net.BCrypt.HashPassword("password"),

                Name = "Johny",

                Role = "User"
            };

            var result = await _service.Login(authUserDto);

            Assert.True(result.Success);

            Assert.Equal(StatusCode.BadRequest, result.StatusCode);
        }
    }
}
