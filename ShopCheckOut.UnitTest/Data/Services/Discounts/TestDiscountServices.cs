using AutoMapper;
using FluentAssertions;
using Moq;
using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Data.Services.Discounts;
using ShopCheckOut.API.Dtos.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data.Services.Discounts
{
    public class TestDiscountServices
    {
        private readonly Mock<IDiscountRepo> _mockDiscountRepo;
        private readonly Mock<IProductsRepo> _mockProductsRepo;
        private readonly IMapper _mapper;
        private readonly DiscountService _discountService;

        public TestDiscountServices()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddDiscountRequest, DiscountsModel>()
               .ForMember(dest => dest.MinQuantity, opt => opt.MapFrom(src => src.MinQuantity))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
               .IncludeAllDerived();

                cfg.CreateMap<AddDiscountRequest, PercentageDiscount>()
                        .ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage));

                cfg.CreateMap<AddDiscountRequest, FixedPriceDiscount>()
                    .ForMember(dest => dest.FixedPrice, opt => opt.MapFrom(src => src.FixedPrice));

                cfg.CreateMap<AddDiscountRequest, BuyXGetYDiscount>()
                    .ForMember(dest => dest.FreeItem, opt => opt.MapFrom(src => src.FreeItem));

                cfg.CreateMap<ProductDiscountModel, ProductDiscountDto>()
                    .IncludeAllDerived();
            });
            _mapper = config.CreateMapper();
            _mockDiscountRepo = new Mock<IDiscountRepo>();
            _mockProductsRepo = new Mock<IProductsRepo>();
            _discountService = new DiscountService(
                _mockDiscountRepo.Object,
                _mockProductsRepo.Object,
                _mapper);
        }

        [Fact]
        public async Task AddNewDiscount_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var request = new AddDiscountRequest
            {
                Name = "Test Discount",
                ProductSKU = "INVALID_SKU",
                IsActive = true,
                MinQuantity = 1,
                DiscountType = DiscountTypes.Percentage
            };
            _mockProductsRepo.Setup(repo => repo.GetProductIdBySku(request.ProductSKU))
                .ThrowsAsync(new KeyNotFoundException($"Discount {request.ProductSKU} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _discountService.AddNewDiscout(request));
        }

        [Theory]
        [InlineData(DiscountTypes.Percentage)]
        [InlineData(DiscountTypes.FixedPrice)]
        [InlineData(DiscountTypes.BuyXGetY)]
        public async Task AddNewDiscount_ShouldAddNewDiscount(DiscountTypes discountType)
        {
            // Arrange  
            var request = new AddDiscountRequest
            {
                Name = "Test Discount",
                ProductSKU = "SKU",
                IsActive = true,
                MinQuantity = 1,
                DiscountType = discountType
            };
            var product = new ProductsModel { Id = 1, Name = "Test Product" };
            DiscountsModel discount = discountType switch
            {
                DiscountTypes.Percentage => new PercentageDiscount { Id = 1, Name = "Test Discount", Percentage = 10 },
                DiscountTypes.FixedPrice => new FixedPriceDiscount { Id = 1, Name = "Test Discount", FixedPrice = 10 },
                DiscountTypes.BuyXGetY => new BuyXGetYDiscount { Id = 1, Name = "Test Discount", FreeItem = 1 },
                _ => throw new ArgumentOutOfRangeException()
            };
            var productDiscount = new ProductDiscountModel
            {
                ProductId = 1,
                Product = product,
                DiscountId = 1,
                Discount = discount
            };
            _mockProductsRepo.Setup(repo => repo.GetProductIdBySku(request.ProductSKU))
                .ReturnsAsync(product.Id.ToString());
            _mockDiscountRepo.Setup(repo => repo.AddNewDiscout(discount, product.Id))
                .Returns(Task.CompletedTask);
            var discountservice = new DiscountService(_mockDiscountRepo.Object, _mockProductsRepo.Object, _mapper);

            // Act  
            var result = await discountservice.AddNewDiscout(request);
            // Assert
            Assert.Equal(Task.CompletedTask, result);
        }

        [Fact]
        public async Task GetDiscounts_ShouldReturnMappedDiscounts()
        {
            // Arrange
            var discounts = new List<DiscountsModel> {
                new PercentageDiscount { Id = 1, Name="discount 1", Percentage = 10 },
                new PercentageDiscount { Id = 2, Name="discount 2", Percentage = 10 }
            };
            var product = new ProductsModel { Id = 1, Name = "Test Product" };
            var productDiscounts = new List<ProductDiscountModel> {
                new ProductDiscountModel {
                    ProductId = 1,
                    Product = product,
                    DiscountId = 1,
                    Discount=discounts[0] }
            };
            var discountDtos = new List<ProductDiscountDto> {
                new ProductDiscountDto {
                    Product = product
                    , Discount = discounts[0]
                }
            };

            _mockDiscountRepo.Setup(repo => repo.GetAvailableDiscounts()).ReturnsAsync(productDiscounts);

            // Act
            var result = await _discountService.GetDiscounts();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Product.Name.Should().Be(discountDtos[0].Product.Name);
            result[0].Discount.Name.Should().Be(discountDtos[0].Discount.Name);
        }

        [Fact]
        public async Task DeleteDiscount_ShouldCallRepoMethod()
        {
            // Arrange
            int discountId = 1;
            _mockDiscountRepo.Setup(repo => repo.DeleteDiscount(discountId)).Returns(Task.CompletedTask);

            // Act
            await _discountService.DeleteDiscount(discountId);

            // Assert
            _mockDiscountRepo.Verify(repo => repo.DeleteDiscount(discountId), Times.Once);
        }


    }
}
