using ShopCheckOut.API.Data.Orders;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data
{
    public class TestOrderFuncs
    {
        [Fact]
        public async Task TestNewOrder_ok()
        {
            // Arrange
            var service = new OrderService();

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
            var service = new OrderService();
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
            Assert.Equal(860, result.TotalAmount);

        }

        [Fact]
        public async Task TestDeletItemFromOrder_Ok()
        {
            // Arrange
            var service = new OrderService();
            // Act
            var result = await service.DeleteItemFromOrder(1, 1, 1);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(460, result.TotalAmount);
        }

        [Fact]
        public async Task TestDeletItemFromOrder_Error()
        {
            var service = new OrderService();

            await Assert.ThrowsAsync<Exception>(
                async () => await service.DeleteItemFromOrder(1, 10, 1)
                );

        }

        [Fact]
        public async Task TestOrderCheckOut_withInvalidOrder()
        {
            var service = new OrderService();
            await Assert.ThrowsAsync<Exception>(
                async () => await service.OrderCheckOut(10)
                );
        }

        [Fact]
        public async Task TestOrderCheckOut_Ok()
        {
            var service = new OrderService();
            var result = await service.OrderCheckOut(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(560, result.TotalAmount);
        }
    }
}
