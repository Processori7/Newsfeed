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
using Microsoft.AspNetCore.Mvc;

namespace Newsfeed.Services;

public class FileService : IFileService
{
    private readonly ApiDbContext _db;

    private readonly IWebHostEnvironment _environment;

    private readonly Serilog.ILogger _logger;

    public FileService(ApiDbContext db, IWebHostEnvironment environment, Serilog.ILogger logger)
    {
        _db = db;

        _environment = environment;

        _logger = logger;
    }

    //For tests
    public FileService(ApiDbContext object2, Serilog.ILogger object3)
    {

    }

    public async Task<BaseResponse<FileModel>> UploadFile(IFormFile File)
    {
        try
        {
            if (File != null)
            {
                string extension = Path.GetExtension(File.FileName);

                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    return new BaseResponse<FileModel>()
                    {
                        statusCode = 0,

                        Success = false,

                        StatusCode = StatusCode.BadRequest
                    };
                }

                string path = "/Files/" + File.FileName;

                using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                {
                    await File.CopyToAsync(fileStream);
                }

                _logger.Information("File uploaded successfully: " + _environment.WebRootPath + path);

                return new BaseResponse<FileModel>()
                {
                    Data = _environment.WebRootPath + path,

                    statusCode = 1,

                    Success = true,

                    StatusCode = StatusCode.OK
                };
            }
            else
            {
                _logger.Error("An error occurred while loading the file because File=null");

                return new BaseResponse<FileModel>()
                {
                    statusCode = 0,

                    Success = false,

                    StatusCode = StatusCode.BadRequest,
                };
            }
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while uploading the file: " + ex.Message);

            return new BaseResponse<FileModel>()
            {
                statusCode = 0,

                Success = false,

                StatusCode = StatusCode.BadRequest,
            };
        }
    }

    public byte[] DownloadFile(string filename)
    {
        try
        {
            string filePath = Path.Combine(_environment.WebRootPath, "Files", filename);

            var file = new FileInfo(filePath);

            if (!File.Exists(Path.Combine(filePath)))
                throw new FileNotFoundException("File not found.");

            _logger.Information("File download link created successfully");

            return File.ReadAllBytes(filePath);
        }
        catch (Exception ex)
        {
            _logger.Error("Error creating link to file: " + ex.Message);
            return null;
        }
    }
}
