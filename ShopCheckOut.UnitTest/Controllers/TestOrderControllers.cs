using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopCheckOut.API.Controllers;
using ShopCheckOut.API.Data.Orders;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Orders;
using ShopCheckOut.API.Dtos.Products;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Controllers
{
    public class TestOrderControllers
    {
        private readonly Mock<IOrderService> _mockOrdersService;
        private readonly Mock<IProductsService> _mockProductService;
        private readonly IMapper _mockMapper;
        private readonly OrderController _controller;
        public TestOrderControllers()
        {
            _mockOrdersService = new Mock<IOrderService>();
            _mockProductService = new Mock<IProductsService>();
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
            });
            _mockMapper = config.CreateMapper();

            _controller = new OrderController(
                _mockMapper,
                _mockOrdersService.Object,
                _mockProductService.Object
                );
        }

        [Fact]
        public async Task TestNewOrder_ReturnOk_WithoutCustomerID()
        {
            // Arrange
            var expectedOrder = new OrdersModel
            {
                Id = 1,
                TotalAmount = 0.0m
            };
            _mockOrdersService.Setup(service => service.NewOrder(It.IsAny<string>()))
            .ReturnsAsync(expectedOrder);

            var result = (await _controller.NewOrder(null)).Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            result.StatusCode.Should().Be(200);
            var resultData = result.Value as OrderCreateDto;
            resultData.Should().BeOfType<OrderCreateDto>();
            var expectData = new OrderCreateDto { Id = 1, TotalAmount = 0.0m };
            resultData.Should().BeEquivalentTo(expectData);

        }

        [Fact]
        public async Task TestNewOrder_ReturnBadRequest_WhenOrderCreationFails()
        {
            // Arrange
            _mockOrdersService
                .Setup(service => service.NewOrder(It.IsAny<string>()))
                .ReturnsAsync((OrdersModel)null);

            // Act
            var actionResult = await _controller.NewOrder(null);
            var result = actionResult.Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task AddItemToOrder_ReturnsBadRequest_WhenOrderIdOrSkuIsEmpty()
        {
            var request = new AddItemRequest { ItemSKU = "", Quantity = "1" };
            var result = await _controller.AddItemToOrder("", request);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddItemToOrder_ReturnsBadRequest_WhenOrderIdOrQuantityInvalid()
        {
            var request = new AddItemRequest { ItemSKU = "SKU123", Quantity = "invalid" };
            var result = await _controller.AddItemToOrder("invalid", request);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddItemToOrder_ReturnsNotFound_WhenProductDoesNotExist()
        {
            _mockProductService.Setup(p => p.GetProductBySKU(It.IsAny<string>()))
                .ReturnsAsync((ProductsModel)null);

            var request = new AddItemRequest { ItemSKU = "SKU123", Quantity = "2" };
            var result = await _controller.AddItemToOrder("1", request);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddItemToOrder_ReturnsOk_WhenItemAddedSuccessfully()
        {
            var product = new ProductsModel { Id = 1, SKU = "SKU1", Name = "Product1", Category = "Category1", Price = 10.0m, PriceUnit = "kg" };
            var orderItems = new OrderItems { OrderId = 1, Product = product, Quantity = 2 };
            var order = new OrdersModel { Id = 1, OrderDate = new DateTime(2025, 3, 5), OrderItems = new List<OrderItems>(), TotalAmount = 0.0m };

            _mockProductService.Setup(p => p.GetProductBySKU(It.IsAny<string>())).ReturnsAsync(product);
            _mockOrdersService.Setup(o => o.NewOrder(It.IsAny<string>())).ReturnsAsync(order);
            _mockOrdersService.Setup(o => o.AddItemToOrder(It.IsAny<int>(), It.IsAny<ProductsModel>(), It.IsAny<int>()))
            .ReturnsAsync((int orderId, ProductsModel product, int quantity) =>
            {
                var existingOrder = new OrdersModel
                {
                    Id = orderId,
                    OrderItems = new List<OrderItems> {
                        new OrderItems {
                            OrderId = orderId,
                            Product = product,
                            Quantity = quantity
                        }
                    },
                    TotalAmount = product.Price * quantity
                };
                return existingOrder;
            });

            var request = new AddItemRequest { ItemSKU = "SKU1", Quantity = "2" };
            var result = await _controller.AddItemToOrder("1", request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult);
            var items = Assert.IsType<OrderUpdateDto>(okResult.Value);
            Assert.NotNull(items);
            Assert.Equal(1, items.Id);
            Assert.Equal(20.0m, items.TotalAmount);
        }

        [Fact]
        public async Task DeleteItemFromOrder_ReturnsBadRequest_WhenParametersAreMissing()
        {
            var mockRequest = new RemoveItemRequest { Quantity = "0", ItemSku = "" };
            var result = await _controller.RemoveItemFromOrder("", mockRequest);
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task DeleteItemFromOrder_ReturnsBadRequest_WhenInvalidSku()
        {
            // Act
            var mockRequest = new RemoveItemRequest { Quantity = "0", ItemSku = "xyz" };
            var result = await _controller.RemoveItemFromOrder("1", mockRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid product Sku", badRequestResult.Value);
        }
        [Fact]
        public async Task DeleteItemFromOrder_ReturnsBadRequest_WhenInvalidOrderId()
        {
            // Arrange
            var _mockProduct = new MockData().GetMockProducts();
            _mockProductService.Setup(p => p.GetProductIdBySku(It.IsAny<string>())).ReturnsAsync("1");
            _mockOrdersService.Setup(o => o.DeleteItemFromOrder(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((OrdersModel)null);
            // Act
            var mockRequest = new RemoveItemRequest { Quantity = "1", ItemSku = "SKU1" };
            var result = await _controller.RemoveItemFromOrder("0", mockRequest);
            // Assert
            var badRequestResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Order not Found", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteItemFromOrder_ReturnsOk()
        {
            // Arrange
            var _mockProduct = new MockData().GetMockProducts();
            _mockProductService.Setup(p => p.GetProductIdBySku(It.IsAny<string>())).ReturnsAsync("1");
            _mockOrdersService.Setup(o => o.DeleteItemFromOrder(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new OrdersModel { Id = 1, TotalAmount = 80.0m });
            // Act
            var mockRequest = new RemoveItemRequest { Quantity = "1", ItemSku = "SKU1" };
            var result = await _controller.RemoveItemFromOrder("1", mockRequest);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            var items = Assert.IsType<OrderUpdateDto>(okResult.Value);
            Assert.NotNull(items);
            Assert.Equal(1, items.Id);
            Assert.Equal(80.0m, items.TotalAmount);
        }
    }
}
