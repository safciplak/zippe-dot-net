using System.Net;
using Moq;
using Moq.Protected;
using ProductService.Models;
using Xunit;

namespace ProductService.Tests
{
    public class CategoryServiceTest
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly CategoryService _categoryService;

        public CategoryServiceTest()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new System.Uri("http://localhost:8080")
            };
            _categoryService = new CategoryService(_httpClient);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsCategory_WhenCategoryExists()
        {
            var categoryId = 1;
            var category = new Category { Id = categoryId, Name = "test category" };
            var jsonCategory = System.Text.Json.JsonSerializer.Serialize(category);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith($"/categories/{categoryId}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonCategory)
                });

            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal("test category", result.Name);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ThrowsException_WhenCategoryDoesNotExist()
        {
            var categoryId = 1;

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith($"/categories/{categoryId}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var exception = await Assert.ThrowsAsync<Exception>(() => _categoryService.GetCategoryByIdAsync(categoryId));
            Assert.Equal("an error occurred while fetching the category.", exception.Message);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ThrowsException_WhenHttpRequestFails()
        {
            var categoryId = 1;

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith($"/categories/{categoryId}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var exception = await Assert.ThrowsAsync<Exception>(() => _categoryService.GetCategoryByIdAsync(categoryId));
            Assert.Equal("an error occurred while fetching the category.", exception.Message);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ThrowsException_WhenDeserializationFails()
        {
            var categoryId = 1;
            var invalidJson = "{ invalid json }";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith($"/categories/{categoryId}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(invalidJson)
                });

            var exception = await Assert.ThrowsAsync<Exception>(() => _categoryService.GetCategoryByIdAsync(categoryId));
            Assert.Equal("an error occurred while deserializing the category response.", exception.Message);
        }
    }
}
