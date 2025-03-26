using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopCheckOut.API.Controllers;
using ShopCheckOut.API.Data.Services.Products;
using ShopCheckOut.API.Dtos.Products;
using ShopCheckOut.API.Models;
namespace ShopCheckOut.UnitTest.Controllers
{
    public class TestProductControllers
    {
        private Mock<IProductServices> _mockProductServices;
        public TestProductControllers()
        {
            _mockProductServices = new Mock<IProductServices>();
        }

        [Fact]
        public async Task GetProductsByCategory_Ok()
        {
            // Arrange
            var expectedProducts = new List<ProductReadDto>
            {
                new() { Sku = "SKU1", Name = "Product1", Category = "Category1", PriceInfo = "100 per kg" },
                new() { Sku = "SKU3", Name = "Product3", Brand = "Foo", Category = "Category1", PriceInfo = "130 per g" },
                new() { Sku = "SKU4", Name = "Product4", Category = "Category1", PriceInfo = "140 per kg" }
            };

            _mockProductServices.Setup(service => service.GetProductsByCategory("Category1"))
                .ReturnsAsync(expectedProducts);

            // Act
            var sut = new ProductController(_mockProductServices.Object);
            var result = await sut.GetProductsByCategory("Category1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ProductReadDto>>(okResult.Value);
            returnValue.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public async Task GetProductsByCategory_BadRequest()
        {
            // Arrange
            _mockProductServices.Setup(service => service.GetProductsByCategory(It.IsAny<string>()))
                .ReturnsAsync((List<ProductReadDto>)null);

            var sut = new ProductController(_mockProductServices.Object);
            // Act
            var result = (await sut.GetProductsByCategory("")).Result as BadRequestObjectResult;
            // Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().BeEquivalentTo(new ErrorResponse("Category must entry", "Request category"));
        }

        [Fact]
        public async Task GetProductBySKU_OKAsync()
        {
            // Arrange
            _mockProductServices.Setup(service => service.GetProductBySKU("SKU1"))
                .ReturnsAsync(new ProductReadDto { Sku = "SKU1", Name = "Product1", Category = "Category1", PriceInfo = "kg" });

            var sut = new ProductController(_mockProductServices.Object);

            // Act
            var result = (await sut.GetProductBySKU("SKU1")).Result as OkObjectResult;

            // Assert
            result.StatusCode.Should().Be(200);
            var product = result.Value as ProductReadDto;
            product.Name.Should().Be("Product1");
        }

        [Fact]
        public async Task GetProductBySKU_BadRequest()
        {
            // Arrange
            _mockProductServices.Setup(service => service.GetProductBySKU(It.IsAny<string>()))
                .ReturnsAsync((ProductReadDto?)null);
            var sut = new ProductController(_mockProductServices.Object);
            // Act
            var result = (await sut.GetProductBySKU("")).Result as BadRequestObjectResult;
            // Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().BeEquivalentTo(new ErrorResponse("Cannot Get Products", "Request SKU"));
        }

        [Fact]
        public async Task TestAddProduct_OK()
        {
            // Arrange
            var newProduct = new ProductCreateDto { Sku = "SKU6", Name = "Product6", Category = "Category3", Price = 60.0m, PriceUnit = "item" };
            var productsList = new List<ProductsModel>
            {
                new () { Id = 1, Sku = "SKU1", Name = "Product1", Category = "Category1", Price = 100, PriceUnit = "kg" },
                new () { Id = 2, Sku = "SKU2", Name = "Product2", Brand = "MockBrand", Category = "Category2", Price = 120, PriceUnit = "item" },
                new () { Id = 3, Sku = "SKU3", Name = "Product3", Brand = "Foo", Category = "Category1", Price = 130, PriceUnit = "g" },
                new () { Id = 4, Sku = "SKU4", Name = "Product4", Category = "Category1", Price = 140, PriceUnit = "kg" },
                new () { Id = 5, Sku = "SKU5", Name = "Product5", Category = "Category2", Price = 150, PriceUnit = "item" },
            };

            _mockProductServices.Setup(repo => repo.AddProduct(It.IsAny<ProductCreateDto>()))
                .ReturnsAsync(Task.CompletedTask);

            var sut = new ProductController(_mockProductServices.Object);
            // Act
            var addResult = await sut.AddProduct(newProduct) as OkObjectResult;
            // Assert
            addResult.StatusCode.Should().Be(200);
            addResult.Value.Should().BeEquivalentTo(new { message = "Add product success" });
        }

    }
}
