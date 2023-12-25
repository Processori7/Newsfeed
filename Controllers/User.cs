using Newsfeed.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newsfeed.Models.DTO;
using System.Security.Cryptography;
using System.Text;
using Newsfeed.Services;
using System.ComponentModel.DataAnnotations;

namespace Newsfeed.Controllers;

[ApiController]
[Route("v1/user/")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]

    public async Task<IResult> RegisterUser()
    {
        var response = await _userService.GetAll();

        if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
        {
            return Results.Ok(new { response.Data, response.StatusCode, response.Success });
        }

        return Results.BadRequest(new { response.StatusCode, response.Success });
    }

    [HttpGet("{id}")]

    public async Task<IResult> GetUserById(Guid id)
    {
        var response = await _userService.GetUser(id);

        if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
        {
            return Results.Ok(new { response.Data, response.StatusCode, response.Success });
        }

        return Results.BadRequest(new { response.StatusCode, response.Success });
    }

    [HttpGet("info")]

    public async Task<IResult> GetUserInfo()
    {
        var response = await _userService.GetUserInfo();

        if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
        {
            return Results.Ok(new { response.Data, response.StatusCode, response.Success });
        }

        return Results.BadRequest(new { response.StatusCode, response.Success });
    }

    [HttpPut]

    public async Task<IResult> ChangeData(PutUserDTO user)
    {
        var response = await _userService.ChangeData(user);

        if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
        {
            return Results.Ok(new { response.Data, response.StatusCode, response.Success });
        }

        return Results.BadRequest(new { response.StatusCode, response.Success });
    }

    [HttpDelete]

    public async Task<IResult> DeleteUser()
    {
        var response = await _userService.DeleteUser();

        if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
        {
            return Results.Ok(new { response.StatusCode, response.Success });
        }

        return Results.BadRequest(new { response.StatusCode, response.Success });
    }
}
