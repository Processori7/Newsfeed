using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newsfeed.Models.DTO;
using Newsfeed.Models.Enums;
using Newsfeed.Models.Response;
using Newsfeed.Services;
using Xunit;
using PostgresDb.Data;

namespace Newsfeed.Tests.Services
{
    public class FileServiceTests
    {
        private readonly Mock<INewsRepository<FileModel>> _mockRepository;

        private readonly Mock<ApiDbContext> _mockDbContext;

        private readonly Mock<IWebHostEnvironment> _mockEnvironment;

        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        private readonly Mock<Serilog.ILogger> _mockLogger;

        private readonly FileService _service;

        public FileServiceTests()
        {
            _mockRepository = new Mock<INewsRepository<FileModel>>();

            _mockDbContext = new Mock<ApiDbContext>();

            _mockEnvironment = new Mock<IWebHostEnvironment>();

            _mockLogger = new Mock<Serilog.ILogger>();

            _service = new FileService(

            _mockDbContext.Object,

            _mockEnvironment.Object,


            _mockLogger.Object);
        }

        [Fact]
        public async Task UploadFile_InvalidFile_ReturnsBadRequestResponse()
        {
            var fileMock = new Mock<IFormFile>();

            var fileName = "test.doc";

            fileMock.Setup(f => f.FileName).Returns(fileName);

            var result = await _service.UploadFile(fileMock.Object);

            Assert.False(result.Success);

            Assert.Equal(StatusCode.BadRequest, result.StatusCode);

            _mockRepository.Verify(r => r.Create(It.IsAny<FileModel>()), Times.Never);

            _mockLogger.Verify(l => l.Information(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UploadFile_NullFile_ReturnsBadRequestResponse()
        {
            var result = await _service.UploadFile(null);

            Assert.False(result.Success);

            Assert.Equal(StatusCode.BadRequest, result.StatusCode);

            _mockRepository.Verify(r => r.Create(It.IsAny<FileModel>()), Times.Never);

            _mockLogger.Verify(l => l.Information(It.IsAny<string>()), Times.Never);
        }
    }
}
