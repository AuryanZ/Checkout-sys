using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopCheckOut.API.Controllers;
using ShopCheckOut.API.Data.Services.Discounts;
using ShopCheckOut.API.Dtos.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Controllers
{
    public class TestDiscountControllers
    {
        private readonly Mock<IDiscountService> _mockDiscountService;
        private readonly DiscountController _controller;

        public TestDiscountControllers()
        {
            _mockDiscountService = new Mock<IDiscountService>();

            _controller = new DiscountController(_mockDiscountService.Object);
        }

        [Fact]
        public async Task GetDiscounts_ReturnsOkResult_WithListOfDiscounts()
        {
            // Arrange
            var discounts = new List<ProductDiscountDto>
                {
                    new ProductDiscountDto {
                        Product = new ProductsModel{Id=1,Name="abc",},
                        Discount=new BuyXGetYDiscount{Id=1, Name="xxxxxx", IsActive=true } },
                    new ProductDiscountDto {
                        Product = new ProductsModel { Id = 2, Name = "bbbbb" },
                        Discount= new PercentageDiscount{Id =2, Name="ffffff",IsActive=true } }
                };
            _mockDiscountService.Setup(service => service.GetDiscounts()).ReturnsAsync(discounts);

            // Act
            var result = await _controller.GetDiscounts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(discounts);
        }

        [Fact]
        public async Task GetDiscounts_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockDiscountService.Setup(service => service.GetDiscounts())
                .ThrowsAsync(new ApplicationException("Test exception"));

            // Act
            var result = await _controller.GetDiscounts();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().BeEquivalentTo(new ErrorResponse("Internal server error.", "Test exception"));
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
            _mockDiscountService.Setup(service => service.AddNewDiscout(request))
                .ReturnsAsync(Task.CompletedTask);

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

            _mockDiscountService.Setup(service => service.AddNewDiscout(request))
                .ThrowsAsync(new KeyNotFoundException("Discount type not found"));

            // Act
            var result = await _controller.AddDiscount(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().BeEquivalentTo(new ErrorResponse("Discount not created", "Discount type not found"));
        }

        [Fact]
        public async Task DeleteDiscount_ReturnsOkResult_WhenDiscountDeletedSuccessfully()
        {
            // Arrange
            var discountId = 1;
            _mockDiscountService.Setup(service => service.DeleteDiscount(discountId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteDiscount(discountId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(new { message = $"Deleted discount {discountId}" });
        }

        [Fact]
        public async Task DeleteDiscount_ReturnsNotFound_WhenDiscountDeleteFails()
        {
            // Arrange
            var discountId = 10;
            _mockDiscountService.Setup(service => service.DeleteDiscount(discountId))
                .Throws(new KeyNotFoundException($"Discount {discountId} not found"));

            // Act
            var result = await _controller.DeleteDiscount(discountId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().BeEquivalentTo(new ErrorResponse("Discount not deleted", $"Discount {discountId} not found"));
        }
    }
}
