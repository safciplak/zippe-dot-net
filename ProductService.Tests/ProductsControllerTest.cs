using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductService.Controllers;
using ProductService.Models;
using ProductService.Repositories;
using Xunit;

namespace ProductService.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public void GetAll_ReturnsOkResult_WithListOfProducts()
        {
            _mockRepo.Setup(repo => repo.GetAll()).Returns(new List<Product> { new Product(), new Product() });

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProducts = Assert.IsType<List<Product>>(okResult.Value);
            Assert.Equal(2, returnProducts.Count);
        }

        [Fact]
        public void GetById_ReturnsOkResult_WithProduct()
        {
            var productId = 1;
            _mockRepo.Setup(repo => repo.GetById(productId)).Returns(new Product { Id = productId });

            var result = _controller.GetById(productId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(productId, returnProduct.Id);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var productId = 1;
            _mockRepo.Setup(repo => repo.GetById(productId)).Returns((Product)null);

            var result = _controller.GetById(productId);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WithProduct()
        {
            var product = new Product { Id = 1, Name = "test product", Price = 10 };
            _mockRepo.Setup(repo => repo.AddAsync(product)).Returns(Task.CompletedTask);

            var result = await _controller.Create(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnProduct = Assert.IsType<Product>(createdAtActionResult.Value);
            Assert.Equal(product.Id, returnProduct.Id);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenProductIsNull()
        {
            var result = await _controller.Create(null);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("product is null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenProductIsUpdated()
        {
            var productId = 1;
            var product = new Product { Id = productId, Name = "updated product", Price = 20 };
            _mockRepo.Setup(repo => repo.GetById(productId)).Returns(product);
            _mockRepo.Setup(repo => repo.UpdateAsync(product)).Returns(Task.CompletedTask);

            var result = await _controller.Update(productId, product);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var productId = 1;
            var product = new Product { Id = 2, Name = "updated product", Price = 20 };

            var result = await _controller.Update(productId, product);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var productId = 1;
            var product = new Product { Id = productId, Name = "updated product", Price = 20 };
            _mockRepo.Setup(repo => repo.GetById(productId)).Returns((Product)null);

            var result = await _controller.Update(productId, product);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ReturnsNoContent_WhenProductIsDeleted()
        {
            var productId = 1;
            var product = new Product { Id = productId, Name = "Product to Delete", Price = 10 };
            _mockRepo.Setup(repo => repo.GetById(productId)).Returns(product);
            _mockRepo.Setup(repo => repo.Delete(productId));

            var result = _controller.Delete(productId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var productId = 1;
            _mockRepo.Setup(repo => repo.GetById(productId)).Returns((Product)null);

            var result = _controller.Delete(productId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
