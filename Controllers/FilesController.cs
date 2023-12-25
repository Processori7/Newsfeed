using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Newsfeed.Models.DTO;
using Microsoft.AspNetCore.Http;
using Newsfeed.Services.Interfaces;
using PostgresDb.Data;
using Newsfeed.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.StaticFiles;

namespace Newsfeed.Controllers
{
    [ApiController]
    [Route("/v1/file/")]

    public class FileController : ControllerBase
    {
        private readonly ApiDbContext _db;

        public static IWebHostEnvironment _appEnvironment;

        private readonly IFileService _fileService;

        private readonly IWebHostEnvironment _environment;

        public FileController(IFileService fileService, IWebHostEnvironment appEnvironment, ApiDbContext db, IWebHostEnvironment environment)
        {
            _fileService = fileService;

            _appEnvironment = appEnvironment;

            _environment = environment;

            _db = db;
        }

        [HttpPost("uploadFile")]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UploadModel), 400)]

        public async Task<IResult> UploadFile([Required] IFormFile file)
        {
            var response = await _fileService.UploadFile(file);

            if (response.StatusCode == Models.Enums.StatusCode.OK)
            {
                return Results.Ok(new { Data = response.Data, StatusCode = response.StatusCode, Success = true });
            }

            return Results.BadRequest(new { StatusCode = response.StatusCode, Success = false });
        }

        [HttpGet("{filename}")]
        [ProducesResponseType(typeof(FileModel), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 400)]

        public IResult ReturnStream([Required, FromRoute] string filename)
        {
            try
            {
                var imageBytes = _fileService.DownloadFile(filename);

                var mimeType = "image";

                if (imageBytes.Length > 0)
                {
                    return Results.File(imageBytes, mimeType, filename);
                }
                else
                {
                    return Results.NotFound();
                }
            }
            catch (NullReferenceException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
