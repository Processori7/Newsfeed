using Newsfeed.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newsfeed.Models.DTO;
using System.Security.Cryptography;
using System.Text;
using Newsfeed.Services;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Newsfeed.Controllers;

[ApiController]
[Route("v1/news/")]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet]

    public async Task<IActionResult> GetAll([Required] int page, [Required] int limit)
    {
        var response = await _newsService.GetAll(page, limit);

        if (response.Success)
        {
            return Ok(new { response.Data });
        }

        return BadRequest(new { response.Success });
    }

    [HttpPost]

    public async Task<IResult> CreateNews([Required] CreateNewsDTO news)
    {
        var response = await _newsService.CreateNews(news);

        if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
        {
            return Results.Ok(new { response.Data, response.StatusCode, response.Success });
        }

        return Results.BadRequest(new { response.StatusCode, response.Success });
    }

    [HttpPut]

    public async Task<IResult> ChangeNews([Required] int id, [Required] CreateNewsDTO news)
    {
        var response = await _newsService.ChangeNews(id, news);

        if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
        {
            return Results.Ok(new { response.Data, response.StatusCode, response.Success });
        }

        return Results.BadRequest(new { response.StatusCode, response.Success, response.ErrorCode });
    }

    [HttpDelete("{id}")]

    public async Task<IResult> DeleteNews([Required] int id)
    {
        var response = await _newsService.DeleteNews(id);

        if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
        {
            return Results.Ok(new { response.StatusCode, response.Success });
        }

        return Results.BadRequest(new { response.StatusCode, response.Success, response.ErrorCode });
    }

    [HttpGet("find")]

    public async Task<IResult> FindNews(string? author, string? keywords, [Required] int page, [Required] int limit, [FromQuery] List<string?> tags)
    {
        var response = await _newsService.FindNews(author, keywords, page, limit, tags);

        if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
        {
            return Results.Ok(new { response.Success, response.statusCode, response.Data });
        }

        return Results.BadRequest(new { response.StatusCode, response.Success });
    }

    [HttpGet("users/{userId}")]

    public async Task<IResult> GetNewsUser([Required] int page, [Required] int limit, [Required] Guid userId)
    {
        var response = await _newsService.GetNewsUser(page, limit, userId);

        if (response.StatusCode == Newsfeed.Models.Enums.StatusCode.OK)
        {
            return Results.Ok(new { response.Data, response.StatusCode, response.Success });
        }

        return Results.BadRequest(new { response.StatusCode, response.Success });
    }
}
