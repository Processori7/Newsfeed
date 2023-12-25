using Newsfeed.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newsfeed.Models.DTO;
using System.Security.Cryptography;
using System.Text;
using Newsfeed.Services;

namespace Newsfeed.Controllers
{
    [ApiController]
    [Route("v1/auth/")]
    public class UserControllerAuthorize : ControllerBase
    {
        private readonly IUserServiceAuthorize _userService;

        public UserControllerAuthorize(IUserServiceAuthorize userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]

        public async Task<IResult> RegisterUser([FromBody] RegisterUserDTO registerUserDTO)
        {
            var response = await _userService.Register(registerUserDTO);

            if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
            {

                return Results.Ok(new { response.Data, response.StatusCode, response.Success });
            }

            return Results.BadRequest(new { response.StatusCode, response.Success });
        }

        [HttpPost("login")]

        public async Task<IResult> LoginUser([FromBody] AuthUserDTO authUserDTO)
        {
            var response = await _userService.Login(authUserDTO);

            if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
            {
                return Results.Ok(new { response.Data, response.StatusCode, response.Success });
            }

            return Results.BadRequest(new { response.StatusCode, response.Success });
        }
    }
}
