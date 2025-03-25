using FluentAssertions;
using Moq;
using ShopCheckOut.API.Data.Orders;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data
{
    public class TestOrderFuncs
    {
        private readonly Mock<IOrderRepo> _OrderRepo;

        public TestOrderFuncs()
        {
            _OrderRepo = new Mock<IOrderRepo>();
        }

        [Fact]
        public async Task TestNewOrder_ok()
        {
            // Arrange
            _OrderRepo.Setup(service => service.NewOrder(null)).ReturnsAsync(new OrdersModel
            {
                Id = 3,
                CustomerId = null,
                OrderDate = DateTime.Now,
                OrderItems = new List<OrderItems>(),
                TotalAmount = 0
            });

            // Act
            var result = await _OrderRepo.Object.NewOrder(null);
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(3);
            result.TotalAmount.Should().Be(0);
        }

        [Fact]
        public async Task TestAddItemToOrder_ok()
        {
            // Arrange
            _OrderRepo.Setup(service => service.AddItemToOrder(1, It.IsAny<ProductsModel>(), 3)).ReturnsAsync(new OrdersModel
            {
                Id = 1,
                CustomerId = null,
                OrderDate = new DateTime(2025, 3, 5),
                OrderItems = new List<OrderItems>
                {
                    new OrderItems() { Id = 10, OrderId = 1, ProductId = 1, Product = new ProductsModel { Id = 1, Sku = "SKU1", Name = "Product1", Category = "Category1", Price = 100, PriceUnit = "kg" }, Quantity = 3, DiscountName = "10% Off", Saved = 30 }
                },
                TotalAmount = 830,
                TotalSaved = 60
            });

            ProductsModel prduct = new ProductsModel()
            {
                Id = 1,
                Sku = "SKU1",
                Name = "Product1",
                Category = "Category1",
                Price = 100,
                PriceUnit = "kg"
            };
            // Act
            var result = await _OrderRepo.Object.AddItemToOrder(1, prduct, 3);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.TotalAmount.Should().Be(830);
            result.TotalSaved.Should().Be(60);
        }

        [Fact]
        public async Task TestDeletItemFromOrder_Ok()
        {
            // Arrange
            _OrderRepo.Setup(Object => Object.DeleteItemFromOrder(1, 1, 1)).ReturnsAsync(new OrdersModel
            {
                Id = 1,
                CustomerId = null,
                OrderDate = new DateTime(2025, 3, 5),
                OrderItems = new List<OrderItems>
                {
                    new OrderItems() { Id = 10, OrderId = 1, ProductId = 1, Product = new ProductsModel { Id = 1, Sku = "SKU1", Name = "Product1", Category = "Category1", Price = 100, PriceUnit = "kg" }, Quantity = 2, DiscountName = "10% Off", Saved = 20 },
                    new OrderItems() { Id = 11, OrderId = 1, ProductId = 2, Product = new ProductsModel { Id = 2, Sku = "SKU2", Name = "Product2", Category = "Category2", Price = 170, PriceUnit = "kg" }, Quantity = 3, DiscountName = "Buy 2 get 1 free", Saved = 170 }
                },
                TotalAmount = 340,
                TotalSaved = 190
            });
            // Act
            var result = await _OrderRepo.Object.DeleteItemFromOrder(1, 1, 1);
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.TotalAmount.Should().Be(340);
        }

        [Fact]
        public async Task TestDeletItemFromOrder_Error()
        {
            _OrderRepo.Setup(Object => Object.DeleteItemFromOrder(1, 10, 1)).ThrowsAsync(new Exception("Product not found in order"));


            await Assert.ThrowsAsync<Exception>(
                async () => await _OrderRepo.Object.DeleteItemFromOrder(1, 10, 1)
                );

        }

        [Fact]
        public async Task TestOrderCheckOut_withInvalidOrder()
        {
            _OrderRepo.Setup(Object => Object.OrderCheckOut(10))
                .ThrowsAsync(new Exception("Order not found"));
            await Assert.ThrowsAsync<Exception>(
                async () => await _OrderRepo.Object.OrderCheckOut(10)
                );
        }

        [Fact]
        public async Task TestOrderCheckOut_Ok()
        {
            _OrderRepo.Setup(Object => Object.OrderCheckOut(1)).ReturnsAsync(new OrdersModel
            {
                Id = 1,
                CustomerId = null,
                OrderDate = new DateTime(2025, 3, 5),
                OrderItems = new List<OrderItems>
                {
                    new OrderItems() { Id = 10, OrderId = 1, ProductId = 1, Product = new ProductsModel { Id = 1, Sku = "SKU1", Name = "Product1", Category = "Category1", Price = 100, PriceUnit = "kg" }, Quantity = 2, DiscountName = "10% Off", Saved = 20 },
                    new OrderItems() { Id = 11, OrderId = 1, ProductId = 2, Product = new ProductsModel { Id = 2, Sku = "SKU2", Name = "Product2", Category = "Category2", Price = 170, PriceUnit = "kg" }, Quantity = 3, DiscountName = "Buy 2 get 1 free", Saved = 170 }
                },
                TotalAmount = 560,
                TotalSaved = 190
            });
            var result = await _OrderRepo.Object.OrderCheckOut(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(560, result.TotalAmount);
        }
    }
}
