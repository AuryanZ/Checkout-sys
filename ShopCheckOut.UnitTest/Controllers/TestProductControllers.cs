using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopCheckOut.API.Controllers;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Products;
using ShopCheckOut.API.Models;
namespace ShopCheckOut.UnitTest.Controllers
{
    public class TestProductControllers
    {
        private Mock<IProductsService> _mockProductsService;
        private IMapper _mockMapper;
        public TestProductControllers()
        {
            _mockProductsService = new Mock<IProductsService>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Products, ProductReadDto>();
                cfg.CreateMap<ProductCreateDto, Products>();
            });
            _mockMapper = config.CreateMapper();
        }

        [Fact]
        public async Task GetProductsByCategory_Ok()
        {
            // Arrange
            _mockProductsService.Setup(service => service.GetProductsByCategory("Category1"))
                .ReturnsAsync(new List<Products>
                {
                        new() { Id = 1, SKU = "SKU1", Name = "Product1", Category = "Category1", Price = 10.0m, PriceUnit = "kg" },
                        new() { Id = 3, SKU = "SKU3", Name = "Product3", Category = "Category1", Price = 30.0m, PriceUnit = "g" },
                        new() { Id = 4, SKU = "SKU4", Name = "Product4", Category = "Category1", Price = 40.0m, PriceUnit = "kg" },
                });
            var sut = new ProductControllers(_mockProductsService.Object, _mockMapper);

            // Act
            var result = sut.GetProductsByCategory("Category1").Result as OkObjectResult;

            // Assert
            result.StatusCode.Should().Be(200);

            var products = result.Value as List<ProductReadDto>;
            products.Should().OnlyContain(p => p.Category == "Category1");
        }

        [Fact]
        public async Task GetProductsByCategory_BadRequest()
        {
            // Arrange
            _mockProductsService.Setup(service => service.GetProductsByCategory(It.IsAny<string>()))
                .ReturnsAsync((List<Products>)null);

            var sut = new ProductControllers(_mockProductsService.Object, _mockMapper);
            // Act
            var result = await sut.GetProductsByCategory("") as BadRequestObjectResult;
            // Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("No Query Added");
        }

        [Fact]
        public void GetProductBySKU_OK()
        {
            // Arrange
            _mockProductsService.Setup(service => service.GetProductBySKU("SKU1"))
                .ReturnsAsync(new Products { Id = 1, SKU = "SKU1", Name = "Product1", Category = "Category1", Price = 10.0m, PriceUnit = "kg" });
            _mockProductsService.Setup(service => service.GetProductBySKU("SKU3"))
                .ReturnsAsync(new Products { Id = 3, SKU = "SKU3", Name = "Product3", Category = "Category1", Price = 30.0m, PriceUnit = "g" });

            var sut = new ProductControllers(_mockProductsService.Object, _mockMapper);

            // Act
            var result = sut.GetProductBySKU("SKU1").Result as OkObjectResult;
            var result2 = sut.GetProductBySKU("SKU3").Result as OkObjectResult;

            // Assert
            result.StatusCode.Should().Be(200);
            var product = result.Value as ProductReadDto;
            product.Name.Should().Be("Product1");

            result2.StatusCode.Should().Be(200);
            var product2 = result2.Value as ProductReadDto;
            product2.Name.Should().Be("Product3");
        }

        [Fact]
        public async Task GetProductBySKU_BadRequest()
        {
            // Arrange
            _mockProductsService.Setup(service => service.GetProductBySKU(It.IsAny<string>()))
                .ReturnsAsync((Products?)null);
            var sut = new ProductControllers(_mockProductsService.Object, _mockMapper);
            // Act
            var result = await sut.GetProductBySKU("") as BadRequestObjectResult;
            // Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("No SKU Added");
        }

        [Fact]
        public async Task AddProduct_OK()
        {
            // Arrange
            var newProduct = new ProductCreateDto { SKU = "SKU6", Name = "Product6", Category = "Category3", Price = 60.0m, PriceUnit = "item" };
            var productsList = new List<Products>
            {
                new() { Id = 1, SKU = "SKU1", Name = "Product1", Category = "Category1", Price = 10.0m, PriceUnit = "kg" },
                new() { Id = 2, SKU = "SKU2", Name = "Product2", Category = "Category2", Price = 20.0m, PriceUnit = "item" },
                new() { Id = 3, SKU = "SKU3", Name = "Product3", Category = "Category1", Price = 30.0m, PriceUnit = "g" },
                new() { Id = 4, SKU = "SKU4", Name = "Product4", Category = "Category1", Price = 40.0m, PriceUnit = "kg" },
                new() { Id = 5, SKU = "SKU5", Name = "Product5", Category = "Category2", Price = 50.0m, PriceUnit = "item" },
            };

            _mockProductsService.Setup(service => service.AddProduct(It.IsAny<Products>()))
                .ReturnsAsync(true)
                .Callback<Products>(product => productsList.Add(product));
            _mockProductsService.Setup(service => service.GetProducts())
                .ReturnsAsync(productsList);

            var sut = new ProductControllers(_mockProductsService.Object, _mockMapper);
            // Act
            var addResult = await sut.AddProduct(newProduct) as OkObjectResult;
            // Assert
            addResult.StatusCode.Should().Be(200);
            // Act
            var getResult = await sut.GetProducts() as OkObjectResult;
            // Assert
            getResult.StatusCode.Should().Be(200);
            var retrievedProducts = getResult.Value as List<ProductReadDto>;
            retrievedProducts.Should().Contain(p => p.SKU == newProduct.SKU && p.Name == newProduct.Name);
        }

    }
}
