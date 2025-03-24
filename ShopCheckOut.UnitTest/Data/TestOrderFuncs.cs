using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Data.Orders;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data
{
    public class TestOrderFuncs
    {
        private readonly DiscountService _discountService;

        public TestOrderFuncs()
        {
            _discountService = new DiscountService();
        }

        [Fact]
        public async Task TestNewOrder_ok()
        {
            // Arrange
            var service = new OrderService(_discountService);

            // Act
            var result = await service.NewOrder(null);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Equal((decimal)0.0, result.TotalAmount);

        }

        [Fact]
        public async Task TestAddItemToOrder_ok()
        {
            // Arrange
            var service = new OrderService(_discountService);
            ProductsModel prduct = new ProductsModel()
            {
                Id = 1,
                SKU = "SKU1",
                Name = "Product1",
                Category = "Category1",
                Price = 100,
                PriceUnit = "kg"
            };
            // Act
            var result = await service.AddItemToOrder(1, prduct, 3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(830, result.TotalAmount);
            Assert.Equal(60, result.TotalSaved);
        }

        [Fact]
        public async Task TestDeletItemFromOrder_Ok()
        {
            // Arrange
            var service = new OrderService(_discountService);
            // Act
            var result = await service.DeleteItemFromOrder(1, 1, 1); // 10% off
            var result2 = await service.DeleteItemFromOrder(2, 3, 2); // buy 2 get 1 free
            // Assert
            Assert.NotNull(result); // 10% off
            Assert.Equal(1, result.Id); // 10% off
            Assert.Equal(470, result.TotalAmount); // 10% off

            Assert.NotNull(result2); // buy 2 get 1 free
            Assert.Equal(2, result2.Id); // buy 2 get 1 free
            Assert.Equal(510, result2.TotalAmount); // buy 2 get 1 free
        }

        [Fact]
        public async Task TestDeletItemFromOrder_Error()
        {
            var service = new OrderService(_discountService);

            await Assert.ThrowsAsync<Exception>(
                async () => await service.DeleteItemFromOrder(1, 10, 1)
                );

        }

        [Fact]
        public async Task TestOrderCheckOut_withInvalidOrder()
        {
            var service = new OrderService(_discountService);
            await Assert.ThrowsAsync<Exception>(
                async () => await service.OrderCheckOut(10)
                );
        }

        [Fact]
        public async Task TestOrderCheckOut_Ok()
        {
            var service = new OrderService(_discountService);
            var result = await service.OrderCheckOut(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(560, result.TotalAmount);
        }
    }
}
