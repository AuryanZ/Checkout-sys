using AutoMapper;
using FluentAssertions;
using Moq;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Data.Services.Products;
using ShopCheckOut.API.Dtos.Products;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data.Services.Products
{
    public class TestProductServices
    {
        private readonly Mock<IProductsRepo> _mockRepo;
        private readonly IMapper _mapper;
        private readonly ProductServices _service;

        public TestProductServices()
        {
            _mockRepo = new Mock<IProductsRepo>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.
                CreateMap<ProductsModel, ProductReadDto>()
                .ForMember(dest => dest.PriceInfo,
                opt => opt.MapFrom(src => $"{src.Price} per {src.PriceUnit}"));
                cfg.CreateMap<ProductCreateDto, ProductsModel>();
            });
            _mapper = config.CreateMapper();
            _service = new ProductServices(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetProductBySKU_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            var product = new ProductsModel { Id = 1, Name = "Test Product", Sku = "12345" };
            _mockRepo.Setup(repo => repo.GetProductBySKU("12345")).ReturnsAsync(product);

            // Act
            var result = await _service.GetProductBySKU("12345");

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(product.Name);
        }

        [Fact]
        public async Task GetProductBySKU_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetProductBySKU("99999"))
                .ThrowsAsync(new KeyNotFoundException("Product not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetProductBySKU("99999"));
        }
        [Fact]
        public async Task GetProducts_ReturnsList_WhenProductsExist()
        {
            // Arrange
            var products = new List<ProductsModel> { new ProductsModel { Id = 1, Name = "Product1" } };
            _mockRepo.Setup(repo => repo.GetProducts()).ReturnsAsync(products);

            // Act
            var result = await _service.GetProducts();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Name.Should().Be(products[0].Name);
        }

        [Fact]
        public async Task GetProducts_ThrowsException_WhenNoProductsExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetProducts()).ReturnsAsync((List<ProductsModel>)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetProducts());
        }

        [Fact]
        public async Task AddProduct_CallsRepoMethodOnce()
        {
            // Arrange
            var productDto = new ProductCreateDto
            {
                Name = "New Product",
                Sku = "123",
                Category = "Category1",
                Price = 10,
                PriceUnit = "kg"
            };
            _mockRepo.Setup(repo => repo.AddProduct(It.IsAny<ProductsModel>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.AddProduct(productDto);

            // Assert
            _mockRepo.Verify(repo => repo.AddProduct(It.IsAny<ProductsModel>()), Times.Once);
        }
    }
}
