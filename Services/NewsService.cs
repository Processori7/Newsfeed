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
using Newsfeed.Controllers;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Newsfeed.Services;

public class NewsService : INewsService
{
    private readonly INewsRepository<NewsModel> _repository;

    private readonly ApiDbContext _db;

    private readonly Serilog.ILogger _logger;

    public NewsService(INewsRepository<NewsModel> repository, ApiDbContext db, Serilog.ILogger logger)
    {
        _repository = repository;

        _db = db;

        _logger = logger;
    }

    public async Task<BaseResponse<NewsModel>> GetAll(int page, int limit)
    {
        try
        {
            var news = await _repository.GetAll();

            _logger.Information("Getting information about all the news");

            var paginatedNews = news
                .Skip((page - 1) * limit)
                .Take(limit)
                .OrderBy(item => item.Id)
                .ToList();


            return new BaseResponse<NewsModel>()
            {
                Success = true,

                StatusCode = StatusCode.OK,

                statusCode = 1,

                Data = new DataResponse<NewsModel>()
                {
                    Content = paginatedNews,

                    numberOfElements = paginatedNews.Count
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error("Error while executing get request to get list of news: " + ex.Message);

            return new BaseResponse<NewsModel>()
            {
                Success = false,

                StatusCode = StatusCode.BadRequest,

                TimeStamp = DateTime.UtcNow
            };
        }
    }

    public async Task<BaseResponse<NewsModel>> CreateNews(CreateNewsDTO news)
    {
        try
        {
            var IdModel = new NewsModel();

            if (!UserServiceAuthorize.IsAuthorized)
            {
                _logger.Error("News creation error. The user has not been authorized.");

                return new BaseResponse<NewsModel>()
                {
                    Success = false,

                    StatusCode = StatusCode.Unauthorized,

                    ErrorCode = 7
                };
            }
            else if (string.IsNullOrWhiteSpace(news.Description) || string.IsNullOrWhiteSpace(news.Title))
            {
                return new BaseResponse<NewsModel>()
                {
                    Success = false,

                    StatusCode = StatusCode.BadRequest,

                    ErrorCode = 7
                };
            }
            else
            {
                if (string.IsNullOrWhiteSpace(UserServiceAuthorize.userName))
                {
                    return new BaseResponse<NewsModel>()
                    {
                        Success = false,

                        StatusCode = StatusCode.Unauthorized,

                        ErrorCode = 7
                    };
                }
                else
                {
                    foreach (string tag in news.Tags)
                    {
                        if (string.IsNullOrWhiteSpace(tag))
                        {
                            return new BaseResponse<NewsModel>()
                            {
                                Success = false,

                                StatusCode = StatusCode.BadRequest,

                                ErrorCode = 7
                            };
                        }
                    }

                    var newNews = new NewsModel()
                    {
                        Id = IdModel.Id,

                        Description = news.Description,

                        Title = news.Title,

                        Tags = news.Tags,

                        Image = news.Image,

                        UserId = UserServiceAuthorize.userId,

                        UserName = UserServiceAuthorize.userName
                    };

                    await _repository.Create(newNews);

                    _logger.Information($"Headline news: {news.Title} successfully created");

                    var response = new BaseResponse<NewsModel>()
                    {
                        Data = newNews,

                        StatusCode = StatusCode.OK,

                        Success = true
                    };

                    return response;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Error creating news: " + ex.Message);

            return new BaseResponse<NewsModel>()
            {
                Success = false,

                StatusCode = StatusCode.BadRequest,

                TimeStamp = DateTime.Now,
            };
        }
    }

    public async Task<BaseResponse<NewsModel>> ChangeNews(int id, CreateNewsDTO news)
    {
        try
        {
            if (!UserServiceAuthorize.IsAuthorized)
            {
                _logger.Error("Error when changing news. The user has not been authorized.");

                return new BaseResponse<NewsModel>()
                {
                    Success = false,

                    StatusCode = StatusCode.Unauthorized,

                    ErrorCode = 7
                };
            }
            else if (string.IsNullOrWhiteSpace(news.Description) || string.IsNullOrWhiteSpace(news.Title) || id == null)
            {
                return await Task.FromResult(new BaseResponse<NewsModel>()
                {
                    StatusCode = StatusCode.BadRequest
                });
            }
            else
            {
                foreach (string tag in news.Tags)
                {
                    if (string.IsNullOrWhiteSpace(tag))
                    {
                        return new BaseResponse<NewsModel>()
                        {
                            Success = false,

                            StatusCode = StatusCode.BadRequest,

                            ErrorCode = 7
                        };
                    }
                }

                var allNews = await _repository.GetAll();

                var targetNews = allNews.FirstOrDefault(n => n.Id == id);

                if (targetNews != null)
                {
                    if (news.Description == "string" & news.Title == "string")
                    {
                        targetNews.Description = targetNews.Description;

                        targetNews.Title = targetNews.Title;

                        targetNews.Tags = news.Tags;

                        targetNews.Image = news.Image;
                    }
                    else
                    {
                        targetNews.Description = news.Description;

                        targetNews.Title = news.Title;

                        targetNews.Tags = news.Tags;

                        targetNews.Image = news.Image;
                    }

                    await _repository.Update(targetNews);

                    _logger.Information($"Headline news: {news.Title} successfully modified by user: {UserServiceAuthorize.userName} with id: {UserServiceAuthorize.userId} and mail: {UserServiceAuthorize.userEmail}");

                    var response = new BaseResponse<NewsModel>()
                    {
                        Data = targetNews,

                        StatusCode = StatusCode.OK,

                        Success = true
                    };
                    return response;
                }
                else
                {
                    return new BaseResponse<NewsModel>()
                    {
                        Success = false,

                        StatusCode = StatusCode.BadRequest,

                        ErrorCode = 7
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Error when changing news: " + ex.Message);

            return new BaseResponse<NewsModel>()
            {
                Success = false,

                StatusCode = StatusCode.InternalServerError,

                ErrorCode = 6
            };
        }
    }

    public async Task<BaseResponse<NewsModel>> DeleteNews(int id)
    {
        try
        {
            if (!UserServiceAuthorize.IsAuthorized)
            {
                _logger.Error("Error when deleting news. The user has not been authorized.");

                return new BaseResponse<NewsModel>()
                {
                    Success = false,

                    StatusCode = StatusCode.Unauthorized,

                    ErrorCode = 7
                };
            }
            else
            {
                var allNews = await _repository.GetAll();

                var targetNews = allNews.FirstOrDefault(n => n.Id == id);

                var currentUser = UserServiceAuthorize.userId;

                await _repository.Delete(targetNews);

                _logger.Information($"News with id: {id} successfully deleted by user: {UserServiceAuthorize.user} with login: {UserServiceAuthorize.userEmail}");

                var response = new BaseResponse<NewsModel>()
                {
                    StatusCode = StatusCode.OK,

                    Success = true
                };
                return response;
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Error when deleting news from id {id}: " + ex.Message);

            return new BaseResponse<NewsModel>()
            {
                Success = false,

                StatusCode = StatusCode.BadRequest,

                ErrorCode = 7
            };
        }
    }

    public async Task<BaseResponse<NewsModel>> GetNewsUser([Required] int page, [Required] int limit, [Required] Guid userId)
    {
        try
        {
            var news = await _repository.GetAll();

            _logger.Information($"Getting information about all user news with id {userId}");

            var paginatedNews = news
            .Skip((page - 1) * limit)
            .Take(limit)
            .OrderBy(item => item.Id)
            .Where(item => item.UserId == userId)
            .ToList();

            return new BaseResponse<NewsModel>()
            {
                Success = true,

                StatusCode = StatusCode.OK,

                statusCode = 1,

                Data = new DataResponse<NewsModel>()
                {
                    Content = paginatedNews,

                    numberOfElements = paginatedNews.Count
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error($"Error while executing get request to get list of news of user with id: {userId}: " + ex.Message);

            return new BaseResponse<NewsModel>()
            {
                Success = false,

                StatusCode = StatusCode.BadRequest,

                TimeStamp = DateTime.UtcNow
            };
        }
    }

    public async Task<BaseResponse<NewsModel>> FindNews(string author, string keywords, int page, int limit, List<string> tags)
    {
        try
        {
            var news = await _repository.GetAll();

            var filteredNews = news
                    .Where(item => string.IsNullOrEmpty(author) || item.UserName == author || item.UserName.Contains(author))
                    .Where(item => string.IsNullOrEmpty(keywords) || item.Description == keywords || item.Title == keywords || item.Description.Contains(keywords) || item.Title.Contains(keywords))
                    .Where(item => tags.Count == 0 || tags.Any(tag => item.Tags.Contains(tag)))
                    .ToList();

            var paginatedNews = filteredNews
                .Skip((page - 1) * limit)
                .Take(limit)
                .OrderBy(item => item.Id)
                .ToList();

            _logger.Information($"News search completed successfully. Author news {author} with keywords {keywords} and tags {string.Join(", ", tags)} successfully found.");

            return new BaseResponse<NewsModel>()
            {
                Success = true,

                StatusCode = StatusCode.OK,

                statusCode = 1,

                Data = new DataResponse<NewsModel>()
                {
                    Content = paginatedNews,
                    
                    numberOfElements = paginatedNews.Count
                }
            };
        }
        catch (ArgumentException ex)
        {
            _logger.Error($"Error when performing a news search: " + ex.Message);

            return new BaseResponse<NewsModel>()
            {
                Success = false,

                StatusCode = StatusCode.BadRequest,

                TimeStamp = DateTime.UtcNow
            };
        }
    }
}
