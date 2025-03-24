using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopCheckOut.API.Controllers;
using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Controllers
{
    public class TestDiscountControllers
    {
        private readonly Mock<IDiscountService> _mockDiscountService;
        private readonly Mock<IProductsService> _mockProductsService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DiscountController _controller;

        public TestDiscountControllers()
        {
            _mockDiscountService = new Mock<IDiscountService>();
            _mockProductsService = new Mock<IProductsService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new DiscountController(_mockDiscountService.Object, _mockProductsService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetDiscounts_ReturnsOkResult_WithListOfDiscounts()
        {
            // Arrange
            var discounts = new List<ProductDiscountModel>
                {
                    new ProductDiscountModel { ProductId = 1, DiscountId = 1 },
                    new ProductDiscountModel { ProductId = 2, DiscountId = 2 }
                };
            _mockDiscountService.Setup(service => service.GetAvailableDiscounts()).ReturnsAsync(discounts);

            // Act
            var result = (await _controller.GetDiscounts()).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnDiscounts = Assert.IsType<List<ProductDiscountModel>>(okResult.Value);
            Assert.Equal(2, returnDiscounts.Count);
        }

        [Fact]
        public async Task GetDiscounts_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _mockDiscountService.Setup(service => service.GetAvailableDiscounts()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = (await _controller.GetDiscounts()).Result;

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Cannot Get Discounts", errorResponse.Message);
        }

        [Fact]
        public async Task AddDiscount_ReturnsOkResult_WhenDiscountAddedSuccessfully()
        {
            // Arrange
            var request = new AddDiscountRequest
            {
                Name = "10% Off",
                IsActive = true,
                minQuantity = 5,
                DiscountType = DiscountTypes.Percentage,
                Percentage = 10,
                productSKU = "SKU123"
            };
            var discount = new PercentageDiscount { Id = 1, Name = "10% Off", IsActive = true, minQuantity = 5, Percentage = 10 };
            _mockProductsService.Setup(service => service.GetProductIdBySku(request.productSKU)).ReturnsAsync("1");
            _mockMapper.Setup(mapper => mapper.Map(request, request.GetType(), typeof(PercentageDiscount))).Returns(discount);
            _mockDiscountService.Setup(service => service.AddNewDiscout(discount, 1)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddDiscount(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task AddDiscount_ReturnsBadRequest_WhenDiscountTypeNotFound()
        {
            // Arrange
            var request = new AddDiscountRequest
            {
                Name = "10% Off",
                IsActive = true,
                minQuantity = 5,
                DiscountType = (DiscountTypes)999, // Invalid discount type
                Percentage = 10,
                productSKU = "SKU123"
            };

            // Act
            var result = await _controller.AddDiscount(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Console.WriteLine(errorResponse.Message);
            Console.WriteLine(errorResponse.Error);
        }

        [Fact]
        public async Task DeleteDiscount_ReturnsOkResult_WhenDiscountDeletedSuccessfully()
        {
            // Arrange
            var discountId = 1;
            _mockDiscountService.Setup(service => service.DeleteDiscount(discountId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteDiscount(discountId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task DeleteDiscount_ReturnsBadRequest_WhenDiscountDeleteFails()
        {
            // Arrange
            var discountId = 1;
            _mockDiscountService.Setup(service => service.DeleteDiscount(discountId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteDiscount(discountId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Console.WriteLine(errorResponse.Message);
            Console.WriteLine(errorResponse.Error);
        }
    }
}
