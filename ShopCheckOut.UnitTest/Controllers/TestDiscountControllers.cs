using AutoMapper;
using FluentAssertions;
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
        private readonly Mock<IDiscountRepo> _mockDiscountRepo;
        private readonly Mock<IProductsRepo> _mockProductsRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DiscountController _controller;

        public TestDiscountControllers()
        {
            _mockDiscountRepo = new Mock<IDiscountRepo>();
            _mockProductsRepo = new Mock<IProductsRepo>();
            _mockMapper = new Mock<IMapper>();
            _controller = new DiscountController(_mockDiscountRepo.Object, _mockProductsRepo.Object, _mockMapper.Object);
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
            _mockDiscountRepo.Setup(service => service.GetAvailableDiscounts()).ReturnsAsync(discounts);

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
            _mockDiscountRepo.Setup(service => service.GetAvailableDiscounts()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = (await _controller.GetDiscounts()).Result;

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            errorResponse.Message.Should().Be("Cannot Get Discounts");
        }

        [Fact]
        public async Task AddDiscount_ReturnsOkResult_WhenDiscountAddedSuccessfully()
        {
            // Arrange
            var request = new AddDiscountRequest
            {
                Name = "10% Off",
                IsActive = true,
                MinQuantity = 5,
                DiscountType = DiscountTypes.Percentage,
                Percentage = 10,
                ProductSKU = "SKU123"
            };
            var discount = new PercentageDiscount { Id = 1, Name = "10% Off", IsActive = true, MinQuantity = 5, Percentage = 10 };
            _mockProductsRepo.Setup(service => service.GetProductIdBySku(request.ProductSKU)).ReturnsAsync("1");
            _mockMapper.Setup(mapper => mapper.Map(request, request.GetType(), typeof(PercentageDiscount))).Returns(discount);
            _mockDiscountRepo.Setup(service => service.AddNewDiscout(discount, 1));

            // Act
            var result = await _controller.AddDiscount(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(new { Message = "Discount added successfully" });

        }

        [Fact]
        public async Task AddDiscount_ReturnsBadRequest_WhenDiscountTypeNotFound()
        {
            // Arrange
            var request = new AddDiscountRequest
            {
                Name = "10% Off",
                IsActive = true,
                MinQuantity = 5,
                DiscountType = (DiscountTypes)999, // Invalid discount type
                Percentage = 10,
                ProductSKU = "SKU123"
            };

            // Act
            var result = await _controller.AddDiscount(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            errorResponse.Message.Should().Be("Discount not created");
            errorResponse.Error.Should().Be("The given key '999' was not present in the dictionary.");
        }

        [Fact]
        public async Task DeleteDiscount_ReturnsOkResult_WhenDiscountDeletedSuccessfully()
        {
            // Arrange
            var discountId = 1;
            _mockDiscountRepo.Setup(service => service.DeleteDiscount(discountId));

            // Act
            var result = await _controller.DeleteDiscount(discountId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(new { message = $"Deleted discount {discountId}" });
        }

        [Fact]
        public async Task DeleteDiscount_ReturnsBadRequest_WhenDiscountDeleteFails()
        {
            // Arrange
            var discountId = 10;
            _mockDiscountRepo.Setup(service => service.DeleteDiscount(discountId)).Throws(new KeyNotFoundException($"Discount {discountId} not found"));

            // Act
            var result = await _controller.DeleteDiscount(discountId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        }
    }
}
