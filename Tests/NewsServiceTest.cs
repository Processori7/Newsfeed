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
    public class NewsServiceTests
    {
        private readonly Mock<INewsRepository<NewsModel>> _mockRepository;

        private readonly Mock<ApiDbContext> _mockDbContext;

        private readonly Mock<IWebHostEnvironment> _mockEnvironment;

        private readonly Mock<Serilog.ILogger> _mockLogger;

        private readonly NewsService _service;

        public NewsServiceTests()
        {
            _mockRepository = new Mock<INewsRepository<NewsModel>>();

            _mockDbContext = new Mock<ApiDbContext>();

            _mockEnvironment = new Mock<IWebHostEnvironment>();

            _mockLogger = new Mock<Serilog.ILogger>();
        }

        [Fact]
        public async Task GetAll_ReturnsAllNews()
        {
            var newslist = new List<NewsModel>
            {
                new NewsModel { Id = 1, Title = "news 1", Description = "news 1", Tags = new List<string> { "tag1", "tag2", "tag3" } },

                new NewsModel { Id = 2, Title = "news 2", Description = "news 2", Tags = new List<string> { "tag4", "tag5", "tag6" } },

                new NewsModel { Id = 3, Title = "news 3", Description = "news 3", Tags = new List<string> { "tag7", "tag8", "tag9" } }
            };

            _mockRepository.Setup(r => r.GetAll()).ReturnsAsync(newslist);

            var service = new NewsService(_mockRepository.Object, _mockDbContext.Object, _mockLogger.Object);

            var response = await service.GetAll(1, 10);

            Assert.True(response.Success);

            Assert.Equal(StatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateNews_WithValidData_ReturnsUnauthorized()
        {
            var news = new CreateNewsDTO
            {
                Title = "new news",

                Description = "super news",

                Tags = new List<string> { "tag1", "tag2" },

                Image = "image.jpg"
            };

            _mockRepository.Setup(r => r.Create(It.IsAny<NewsModel>())).Verifiable();

            var service = new NewsService(_mockRepository.Object, _mockDbContext.Object, _mockLogger.Object);

            var response = await service.CreateNews(news);

            Assert.False(response.Success);

            Assert.Equal(StatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ChangeNews_WithValidIdAndData_ReturnsUnauthorized()
        {
            var newsId = 1;

            var updatedNews = new CreateNewsDTO
            {
                Title = "Updated News",

                Description = "Super Updated News",

                Tags = new List<string> { "tag1", "tag2" },

                Image = "image.jpg"
            };

            var existingNews = new NewsModel { Id = newsId, Title = "Original News" };

            _mockRepository.Setup(r => r.GetAll()).ReturnsAsync(new List<NewsModel> { existingNews });

            _mockRepository.Setup(r => r.Update(It.IsAny<NewsModel>())).Verifiable();

            var service = new NewsService(_mockRepository.Object, _mockDbContext.Object, _mockLogger.Object);

            var response = await service.ChangeNews(newsId, updatedNews);

            Assert.False(response.Success);

            Assert.Equal(StatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteNews_WithValidId_ReturnsSuccess()
        {
            var newsId = 1;

            var existingNews = new NewsModel { Id = newsId, Title = "News to delete" };

            _mockRepository.Setup(r => r.GetAll()).ReturnsAsync(new List<NewsModel> { existingNews });

            _mockRepository.Setup(r => r.Delete(It.IsAny<NewsModel>())).Verifiable();

            var service = new NewsService(_mockRepository.Object, _mockDbContext.Object, _mockLogger.Object);

            var response = await service.DeleteNews(newsId);

            Assert.False(response.Success);

            Assert.Equal(StatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetNewsUser_WithValidUserId_ReturnsNewsFromUser()
        {
            var userId = Guid.NewGuid();

            var userNewsList = new List<NewsModel>
            {
                new NewsModel { Id = 1, Title = "News 1", Description = "News 1", Tags = new List<string> { "tag1", "tag2", "tag3" }, UserId = userId },

                new NewsModel { Id = 2, Title = "News 2", Description = "News 2", Tags = new List<string> { "tag4", "tag5", "tag6" }, UserId = userId },

                new NewsModel { Id = 3, Title = "News 3", Description = "News 3", Tags = new List<string> { "tag7", "tag8", "tag9" }, UserId = Guid.NewGuid() }
            };

            _mockRepository.Setup(r => r.GetAll()).ReturnsAsync(userNewsList);

            var service = new NewsService(_mockRepository.Object, _mockDbContext.Object, _mockLogger.Object);

            var response = await service.GetNewsUser(1, 10, userId);

            Assert.True(response.Success);
        }

        [Fact]
        public async Task FindNews_WithValidParameters_ReturnsMatchingNews()
        {
            var newsList = new List<NewsModel>
            {
                new NewsModel { Id = 1, Title = "News 1", UserName = "Author1", Description = "Description1", Tags = new List<string> { "Tag1" } },

                new NewsModel { Id = 2, Title = "News 2", UserName = "Author2", Description = "Description2", Tags = new List<string> { "Tag2" } },

                new NewsModel { Id = 3, Title = "News 3", UserName = "Author3", Description = "Description3", Tags = new List<string> { "Tag3" } },

                new NewsModel { Id = 4, Title = "News 4", UserName = "Author1", Description = "Description4", Tags = new List<string> { "Tag4" } },
            };

            _mockRepository.Setup(r => r.GetAll()).ReturnsAsync(newsList);

            var service = new NewsService(_mockRepository.Object, _mockDbContext.Object, _mockLogger.Object);

            var response = await service.FindNews("Author1", "Description1", 1, 10, new List<string> { "Tag1" });

            var model = new NewsModel();

            Assert.True(response.Success);

            Assert.Equal(StatusCode.OK, response.StatusCode);
        }
    }
}
