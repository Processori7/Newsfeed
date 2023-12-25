using Microsoft.AspNetCore.Mvc;
using Newsfeed.Models.DTO;
using Newsfeed.Models.Response;
using static Newsfeed.Controllers.FileController;

namespace Newsfeed.Services.Interfaces;

public interface IFileService
{
    byte[] DownloadFile(string fileName);

    Task<BaseResponse<FileModel>> UploadFile(IFormFile File);
}
