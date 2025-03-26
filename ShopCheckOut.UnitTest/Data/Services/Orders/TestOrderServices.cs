using AutoMapper;
using FluentAssertions;
using Moq;
using ShopCheckOut.API.Data.Orders;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Data.Services.Orders;
using ShopCheckOut.API.Dtos.Orders;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data.Services.Orders
{
    public class TestOrderServices
    {
        private readonly Mock<IOrderRepo> _mockOrderRepo;
        private readonly Mock<IProductsRepo> _mockProductsRepo;
        private readonly Mapper _mapper;
        private readonly OrderService _orderService;
        public TestOrderServices()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrdersModel, OrderCreateDto>();

                cfg.CreateMap<OrderItems, ItemUpdateDto>()
                    .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

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
            _mapper = new Mapper(config);
            _mockOrderRepo = new Mock<IOrderRepo>();
            _mockProductsRepo = new Mock<IProductsRepo>();
            _orderService = new OrderService(_mockOrderRepo.Object, _mockProductsRepo.Object, _mapper);
        }

        [Fact]
        public async Task AddItemToOrder_ShouldReturnOrderUpdateDto()
        {
            // Arrange
            var orderId = "1";
            var request = new AddItemRequest { ItemSku = "SKU2", Quantity = "2" };
            var product = new ProductsModel
            {
                Id = 1,
                Sku = "SKU1",
                Name = "Product1",
                Category = "Category1",
                Price = 100,
                PriceUnit = "kg"
            };
            List<OrderItems> mockOrderItem = new List<OrderItems>()
            {
                new OrderItems() { Id = 10, OrderId = 1, ProductId = 1,  Quantity = 2 },
                new OrderItems() { Id = 11, OrderId = 4, ProductId = 4, Quantity = 1 },
                new OrderItems() { Id = 12, OrderId = 2, ProductId = 2, Quantity = 1 },
            };
            var order = new OrdersModel { Id = 1, OrderItems = mockOrderItem, TotalAmount = 460 };

            _mockProductsRepo.Setup(repo => repo.GetProductBySKU(request.ItemSku))
                .ReturnsAsync(product);
            _mockOrderRepo.Setup(repo => repo.AddItemToOrder(1, product, 2))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.AddItemToOrder(orderId, request);

            // Assert
            result.Should().BeOfType<OrderUpdateDto>();
            result.OrderItems.Should().NotBeNull();
            result.TotalAmount.Should().Be(460);
            result.OrderItems.Count.Should().Be(3);
            _mockProductsRepo.Verify(repo => repo.GetProductBySKU("SKU2"), Times.Once);
            _mockOrderRepo.Verify(repo => repo.AddItemToOrder(1, product, 2), Times.Once);
        }

        [Fact]
        public async Task NewOrder_ShouldReturnOrderCreateDto()
        {
            // Arrange
            var newOrder = new OrdersModel { Id = 1, CustomerId = "123", OrderDate = DateTime.Now, TotalAmount = 0 };
            _mockOrderRepo.Setup(repo => repo.NewOrder(It.IsAny<OrdersModel>()))
                .ReturnsAsync(newOrder);

            // Act
            var result = await _orderService.NewOrder("123");

            // Assert
            result.Should().BeOfType<OrderCreateDto>();
            result.TotalAmount.Should().Be(0);

        }

        [Fact]
        public async Task OrderCheckOut_ShouldReturnOrderCheckoutDto()
        {
            // Arrange
            var order = new OrdersModel { Id = 1, TotalAmount = 50 };
            _mockOrderRepo.Setup(repo => repo.OrderCheckOut(1))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.OrderCheckOut("1");

            // Assert
            result.Should().BeOfType<OrderCheckoutDto>();
            result.TotalAmount.Should().Be(50);
        }

        [Fact]
        public async Task RemoveItemFromOrder_ShouldReturnOrderUpdateDto()
        {
            // Arrange
            var orderId = "1";
            var request = new RemoveItemRequest { ItemSku = "SKU1", Quantity = "1" };
            var product = new ProductsModel
            {
                Id = 1,
                Sku = "SKU1",
                Name = "Product1",
                Category = "Category1",
                Price = 100,
                PriceUnit = "kg"
            };
            List<OrderItems> mockOrderItem = new List<OrderItems>()
            {
                new OrderItems() { Id = 10, OrderId = 1, ProductId = 1,  Quantity = 6 },
                new OrderItems() { Id = 11, OrderId = 4, ProductId = 4, Quantity = 1 },
                new OrderItems() { Id = 12, OrderId = 2, ProductId = 2, Quantity = 1 },
            };
            var order = new OrdersModel { Id = 1, OrderItems = mockOrderItem, TotalAmount = 460 };

            _mockProductsRepo.Setup(repo => repo.GetProductBySKU(request.ItemSku))
                .ReturnsAsync(product);
            _mockOrderRepo.Setup(repo => repo.RemoveItemFromOrder(1, product.Id, 1))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.RemoveItemFromOrder(orderId, request);

            // Assert
            result.Should().BeOfType<OrderUpdateDto>();
            result.OrderItems.Should().NotBeNull();
            result.TotalAmount.Should().Be(460);
            _mockOrderRepo.Verify(repo => repo.RemoveItemFromOrder(1, product.Id, 1), Times.Once);
            _mockProductsRepo.Verify(repo => repo.GetProductBySKU("SKU1"), Times.Once);
        }

    }
}
