using Newsfeed.Models.DTO;
using Newsfeed.Models.Response;
using System.ComponentModel.DataAnnotations;

namespace Newsfeed.Services.Interfaces;

public interface INewsService
{
    Task<BaseResponse<NewsModel>> GetAll([Required] int page, [Required] int limit);

    Task<BaseResponse<NewsModel>> CreateNews(CreateNewsDTO news);

    Task<BaseResponse<NewsModel>> ChangeNews([Required] int id, [Required] CreateNewsDTO news);

    Task<BaseResponse<NewsModel>> DeleteNews([Required] int id);

    Task<BaseResponse<NewsModel>> GetNewsUser([Required] int page, [Required] int limit, [Required] Guid userId);

    Task<BaseResponse<NewsModel>> FindNews(string author, string keywords, [Required] int page, [Required] int limit, List<string> tags);
}
