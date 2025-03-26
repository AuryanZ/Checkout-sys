using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopCheckOut.API.Controllers;
using ShopCheckOut.API.Data.Services.Orders;
using ShopCheckOut.API.Dtos.Orders;
using ShopCheckOut.API.Dtos.Products;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Controllers
{
    public class TestOrderControllers
    {
        private readonly Mock<IOrderService> _mockOrdersService;
        private readonly OrderController _controller;
        public TestOrderControllers()
        {
            _mockOrdersService = new Mock<IOrderService>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductsModel, ProductReadDto>()
                .ForMember(dest => dest.PriceInfo,
                opt => opt.MapFrom(src => $"{src.Price} per {src.PriceUnit}"));

                cfg.CreateMap<OrdersModel, OrderCreateDto>();

                cfg.CreateMap<OrderItems, ItemUpdateDto>()
                    .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

                cfg.CreateMap<OrdersModel, OrderUpdateDto>()
                    .ForMember(dest => dest.OrderItems,
                               opt => opt.MapFrom(src => src.OrderItems))
                    .ForMember(dest => dest.TotalAmount,
                               opt => opt.MapFrom(src => src.TotalAmount));
                cfg.CreateMap<OrdersModel, OrderCheckoutDto>()
                .ForMember(dest => dest.OrderItems,
                           opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.TotalAmount,
                           opt => opt.MapFrom(src => src.TotalAmount));
            });

            _controller = new OrderController(_mockOrdersService.Object);
        }

        [Fact]
        public async Task TestNewOrder_ReturnOk_WithoutCustomerID()
        {
            // Arrange
            var expectedOrder = new OrderCreateDto
            {
                Id = 1,
                TotalAmount = 0
            };
            _mockOrdersService.Setup(service => service.NewOrder(It.IsAny<string>()))
            .ReturnsAsync(expectedOrder);
            // Act
            var result = (await _controller.NewOrder(null)).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            result.StatusCode.Should().Be(200);
            var resultData = result.Value as OrderCreateDto;
            resultData.Should().BeEquivalentTo(expectedOrder);

        }

        [Fact]
        public async Task TestNewOrder_ReturnBadRequest_WhenOrderCreationFails()
        {
            // Arrange
            _mockOrdersService
                .Setup(service => service.NewOrder(It.IsAny<string>()))
                .ThrowsAsync(new ApplicationException("Order creation failed"));

            // Act
            var actionResult = await _controller.NewOrder(null);
            var result = actionResult.Result as ObjectResult;

            // Assert
            result.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task AddItemToOrder_ReturnsBadRequest_WhenOrderIdOrSkuIsEmpty()
        {
            // Arrange
            var request = new AddItemRequest { ItemSku = "", Quantity = "1" };
            _mockOrdersService.Setup(o => o.AddItemToOrder(It.IsAny<string>(), It.IsAny<AddItemRequest>()))
                .ThrowsAsync(new ArgumentException("Invalid request data"));
            // Act
            var result = (await _controller.AddItemToOrder("", request)).Result as ObjectResult;
            // Assert
            result.StatusCode.Should().Be(400);
            _mockOrdersService.Verify(o => o.AddItemToOrder(It.IsAny<string>(), It.IsAny<AddItemRequest>()), Times.Once);

        }

        [Fact]
        public async Task AddItemToOrder_ReturnsBadRequest_WhenQuantityInvalid()
        {
            // Arrange
            var request = new AddItemRequest { ItemSku = "SKU123", Quantity = "invalid" };
            _mockOrdersService.Setup(o => o.AddItemToOrder(It.IsAny<string>(), It.IsAny<AddItemRequest>()))
                .ThrowsAsync(new ArgumentException("Invalid request data"));
            // Act
            var result = (await _controller.AddItemToOrder("invalid", request)).Result as ObjectResult;
            // Assert
            result.StatusCode.Should().Be(400);
            _mockOrdersService.Verify(o => o.AddItemToOrder(It.IsAny<string>(), It.IsAny<AddItemRequest>()), Times.Once);
        }

        [Fact]
        public async Task AddItemToOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            _mockOrdersService.Setup(o => o.AddItemToOrder(It.IsAny<string>(), It.IsAny<AddItemRequest>()))
                .ThrowsAsync(new KeyNotFoundException("Order not found."));
            var request = new AddItemRequest { ItemSku = "SKU123", Quantity = "2" };

            // Act
            var result = (await _controller.AddItemToOrder("100", request)).Result as ObjectResult;

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            result.Value.Should().BeEquivalentTo(new ErrorResponse("Order not found.", "Order not found."));
        }

        [Fact]
        public async Task AddItemToOrder_ReturnsOk_WhenItemAddedSuccessfully()
        {
            // Arrange
            var product = new ProductReadDto { Sku = "SKU1", Name = "Product1", Category = "Category1" };
            var order = new OrdersModel { Id = 1, OrderDate = new DateTime(2025, 3, 5), OrderItems = new List<OrderItems>(), TotalAmount = 0 };

            _mockOrdersService.Setup(o => o.AddItemToOrder(It.IsAny<string>(), It.IsAny<AddItemRequest>()))
                .ReturnsAsync(new OrderUpdateDto
                {
                    Id = 1,
                    OrderItems = new List<ItemUpdateDto> { new ItemUpdateDto { Product = product, Quantity = 2 } },
                    TotalAmount = 200
                });
            var request = new AddItemRequest { ItemSku = "SKU1", Quantity = "2" };

            // Act
            var result = (await _controller.AddItemToOrder("1", request)).Result as ObjectResult;

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.Value.Should().BeEquivalentTo(new OrderUpdateDto
            {
                Id = 1,
                OrderItems = new List<ItemUpdateDto> { new ItemUpdateDto { Product = product, Quantity = 2 } },
                TotalAmount = 200
            });
        }

        [Fact]
        public async Task DeleteItemFromOrder_ReturnsBadRequest_WhenParametersAreMissing()
        {
            // Arrange
            _mockOrdersService.Setup(o => o.RemoveItemFromOrder(It.IsAny<string>(), It.IsAny<RemoveItemRequest>()))
                .ThrowsAsync(new ArgumentException("Invalid request data"));
            var mockRequest = new RemoveItemRequest { Quantity = "0", ItemSku = "" };
            // Act
            var result = (await _controller.RemoveItemFromOrder("", mockRequest)).Result as ObjectResult;

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockOrdersService.Verify(o => o.RemoveItemFromOrder(It.IsAny<string>(), It.IsAny<RemoveItemRequest>()), Times.Once);
        }

        [Fact]
        public async Task DeleteItemFromOrder_ReturnsBadRequest_WhenInvalidSku()
        {
            // Arrange
            var mockRequest = new RemoveItemRequest { Quantity = "0", ItemSku = "xyz" };
            _mockOrdersService.Setup(o => o.RemoveItemFromOrder(It.IsAny<string>(), It.IsAny<RemoveItemRequest>()))
                .ThrowsAsync(new ArgumentException("Invalid request data."));
            // Act
            var result = (await _controller.RemoveItemFromOrder("1", mockRequest)).Result as ObjectResult;

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockOrdersService.Verify(o => o.RemoveItemFromOrder(It.IsAny<string>(), It.IsAny<RemoveItemRequest>()), Times.Once);
        }

        [Fact]
        public async Task DeleteItemFromOrder_ReturnsBadRequest_WhenInvalidOrderId()
        {
            // Arrange
            _mockOrdersService.Setup(o => o.RemoveItemFromOrder(It.IsAny<string>(), It.IsAny<RemoveItemRequest>()))
                .ThrowsAsync(new ArgumentException("Invalid order ID format"));
            // Act
            var mockRequest = new RemoveItemRequest { Quantity = "1", ItemSku = "SKU1" };
            var result = (await _controller.RemoveItemFromOrder("0", mockRequest)).Result as ObjectResult;
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockOrdersService.Verify(o => o.RemoveItemFromOrder(It.IsAny<string>(), It.IsAny<RemoveItemRequest>()), Times.Once);
        }

        [Fact]
        public async Task DeleteItemFromOrder_ReturnsOk()
        {
            // Arrange
            List<ItemUpdateDto> mockOrderItem = new List<ItemUpdateDto>()
            {
                new ItemUpdateDto() { Product = new ProductReadDto{ Sku="sku1" }, DiscountName="", Quantity=1, Saved=0 },
                new ItemUpdateDto() { Product = new ProductReadDto{ Sku="sku3" }, DiscountName="", Quantity=4, Saved=0 },
                new ItemUpdateDto() { Product = new ProductReadDto{ Sku="sku2" }, DiscountName="", Quantity=3, Saved=0 },
            };
            _mockOrdersService.Setup(o => o.RemoveItemFromOrder(It.IsAny<string>(), It.IsAny<RemoveItemRequest>()))
                .ReturnsAsync(new OrderUpdateDto { Id = 1, OrderItems = mockOrderItem, TotalAmount = 460, TotalSaved = 10 });

            // Act
            var mockRequest = new RemoveItemRequest { Quantity = "1", ItemSku = "sku1" };
            var result = (await _controller.RemoveItemFromOrder("1", mockRequest)).Result as ObjectResult;
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.Value.Should().BeEquivalentTo(new OrderUpdateDto { Id = 1, OrderItems = mockOrderItem, TotalAmount = 460, TotalSaved = 10 });
            _mockOrdersService.Verify(o => o.RemoveItemFromOrder(It.IsAny<string>(), It.IsAny<RemoveItemRequest>()), Times.Once);
        }

        [Fact]
        public async Task TestCheckOutOrder_RetrunBadRequest_WhenOrderIdisEmpty()
        {
            // Arrange
            _mockOrdersService.Setup(o => o.OrderCheckOut(It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException("Invalid order ID"));
            // Act
            var result = (await _controller.CheckOutOrder("")).Result as ObjectResult;
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockOrdersService.Verify(o => o.OrderCheckOut(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task TestCheckOutOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            _mockOrdersService.Setup(o => o.OrderCheckOut(It.IsAny<string>()))
                .ThrowsAsync(new KeyNotFoundException("Order not found."));
            // Act
            var result = (await _controller.CheckOutOrder("0")).Result as ObjectResult;
            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            _mockOrdersService.Verify(_mockOrdersService => _mockOrdersService.OrderCheckOut(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task TestCheckOutOrder_Success()
        {
            // Arrange
            List<ItemUpdateDto> mockOrderItem = new List<ItemUpdateDto>()
            {
                new ItemUpdateDto() { Product = new ProductReadDto{ Sku="sku1" }, DiscountName="", Quantity=1, Saved=0 },
                new ItemUpdateDto() { Product = new ProductReadDto{ Sku="sku3" }, DiscountName="", Quantity=4, Saved=0 },
                new ItemUpdateDto() { Product = new ProductReadDto{ Sku="sku2" }, DiscountName="", Quantity=3, Saved=0 },
            };
            _mockOrdersService.Setup(o => o.OrderCheckOut(It.IsAny<string>()))
                .ReturnsAsync(new OrderCheckoutDto { Id = 1, OrderItems = mockOrderItem, TotalAmount = 80 });
            // Act
            var result = (await _controller.CheckOutOrder("1")).Result as ObjectResult;
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.Value.Should().BeEquivalentTo(new OrderCheckoutDto { Id = 1, OrderItems = mockOrderItem, TotalAmount = 80 });
            _mockOrdersService.Verify(o => o.OrderCheckOut(It.IsAny<string>()), Times.Once);
        }
    }
}
