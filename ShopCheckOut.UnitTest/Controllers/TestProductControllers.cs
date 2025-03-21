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
                cfg.
                CreateMap<ProductsModel, ProductReadDto>()
                .ForMember(dest => dest.PriceInfo,
                opt => opt.MapFrom(src => $"{src.Price} per {src.PriceUnit}"));
                cfg.CreateMap<ProductCreateDto, ProductsModel>();
            });
            _mockMapper = config.CreateMapper();
        }

        [Fact]
        public async Task GetProductsByCategory_Ok()
        {
            // Arrange
            _mockProductsService.Setup(service => service.GetProductsByCategory("Category1"))
                .ReturnsAsync(new List<ProductsModel>
                {
                        new() { Id = 1, SKU = "SKU1", Name = "Product1", Category = "Category1", Price = 10.0m, PriceUnit = "kg" },
                        new() { Id = 3, SKU = "SKU3", Name = "Product3", Category = "Category1", Price = 30.0m, PriceUnit = "g" },
                        new() { Id = 4, SKU = "SKU4", Name = "Product4", Category = "Category1", Price = 40.0m, PriceUnit = "kg" },
                });
            var sut = new ProductController(_mockProductsService.Object, _mockMapper);

            // Act
            var result = (await sut.GetProductsByCategory("Category1")).Result as OkObjectResult;

            // Assert
            result.StatusCode.Should().Be(200);

            var products = result.Value as List<ProductReadDto>;
            products.Should().OnlyContain(p => p.Category == "Category1");
            products.Should().BeEquivalentTo(
                new List<ProductReadDto> {
                new() { SKU = "SKU1", Name = "Product1", Category = "Category1", PriceInfo = "10.0 per kg" },
                new() {  SKU = "SKU3", Name = "Product3", Category = "Category1", PriceInfo = "30.0 per g"},
                new() {  SKU = "SKU4", Name = "Product4", Category = "Category1", PriceInfo = "40.0 per kg" }
                });
        }

        [Fact]
        public async Task GetProductsByCategory_BadRequest()
        {
            // Arrange
            _mockProductsService.Setup(service => service.GetProductsByCategory(It.IsAny<string>()))
                .ReturnsAsync((List<ProductsModel>)null);

            var sut = new ProductController(_mockProductsService.Object, _mockMapper);
            // Act
            var result = (await sut.GetProductsByCategory("")).Result as BadRequestObjectResult;
            // Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("No Query Added");
        }

        [Fact]
        public async Task GetProductBySKU_OKAsync()
        {
            // Arrange
            _mockProductsService.Setup(service => service.GetProductBySKU("SKU1"))
                .ReturnsAsync(new ProductsModel { Id = 1, SKU = "SKU1", Name = "Product1", Category = "Category1", Price = 10.0m, PriceUnit = "kg" });
            _mockProductsService.Setup(service => service.GetProductBySKU("SKU3"))
                .ReturnsAsync(new ProductsModel { Id = 3, SKU = "SKU3", Name = "Product3", Category = "Category1", Price = 30.0m, PriceUnit = "g" });

            var sut = new ProductController(_mockProductsService.Object, _mockMapper);

            // Act
            var result = (await sut.GetProductBySKU("SKU1")).Result as OkObjectResult;
            var result2 = (await sut.GetProductBySKU("SKU3")).Result as OkObjectResult;

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
                .ReturnsAsync((ProductsModel?)null);
            var sut = new ProductController(_mockProductsService.Object, _mockMapper);
            // Act
            var result = (await sut.GetProductBySKU("")).Result as BadRequestObjectResult;
            // Assert
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("No SKU Added");
        }

        [Fact]
        public async Task TestAddProduct_OK()
        {
            // Arrange
            var newProduct = new ProductCreateDto { SKU = "SKU6", Name = "Product6", Category = "Category3", Price = 60.0m, PriceUnit = "item" };
            var productsList = new List<ProductsModel>
            {
                new() { Id = 1, SKU = "SKU1", Name = "Product1", Category = "Category1", Price = 10.0m, PriceUnit = "kg" },
                new() { Id = 2, SKU = "SKU2", Name = "Product2", Category = "Category2", Price = 20.0m, PriceUnit = "item" },
                new() { Id = 3, SKU = "SKU3", Name = "Product3", Category = "Category1", Price = 30.0m, PriceUnit = "g" },
                new() { Id = 4, SKU = "SKU4", Name = "Product4", Category = "Category1", Price = 40.0m, PriceUnit = "kg" },
                new() { Id = 5, SKU = "SKU5", Name = "Product5", Category = "Category2", Price = 50.0m, PriceUnit = "item" },
            };

            _mockProductsService.Setup(service => service.AddProduct(It.IsAny<ProductsModel>()))
                .ReturnsAsync(true)
                .Callback<ProductsModel>(product => productsList.Add(product));
            _mockProductsService.Setup(service => service.GetProducts())
                .ReturnsAsync(productsList);

            var sut = new ProductController(_mockProductsService.Object, _mockMapper);
            // Act
            var addResult = await sut.AddProduct(newProduct) as OkObjectResult;
            // Assert
            addResult.StatusCode.Should().Be(200);
            // Act
            var getResult = (await sut.GetProducts()).Result as OkObjectResult;
            // Assert
            getResult.StatusCode.Should().Be(200);
            var retrievedProducts = getResult.Value as List<ProductReadDto>;
            retrievedProducts.Should().Contain(p => p.SKU == newProduct.SKU && p.Name == newProduct.Name);
        }

    }
}
