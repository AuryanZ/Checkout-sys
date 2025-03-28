using FluentAssertions;
using ShopCheckOut.API.Data.Orders;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data.Orders
{
    public class TestOrderFuncs
    {
        private readonly OrderRepo _OrderRepo = new OrderRepo();

        [Fact]
        public async Task TestNewOrder_ok()
        {
            // Arrange
            var newOrder = new OrdersModel
            {
                CustomerId = null,
                OrderDate = DateTime.Now,
                OrderItems = new List<OrderItems>(),
                TotalAmount = 0
            };
            // Act
            var result = await _OrderRepo.NewOrder(newOrder);
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(3);
            result.TotalAmount.Should().Be(0);
        }

        //[Fact]
        //public async Task TestAddItemToOrder_ok()
        //{
        //    // Arrange
        //    ProductsModel prduct = new()
        //    {
        //        Id = 1,
        //        Sku = "SKU1",
        //        Name = "Product1",
        //        Category = "Category1",
        //        Price = 100,
        //        PriceUnit = "kg"
        //    };
        //    // Act
        //    var result = await _OrderRepo.AddItemToOrder(1, prduct, 3);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Id.Should().Be(1);
        //    result.TotalAmount.Should().Be(830);
        //    result.TotalSaved.Should().Be(60);
        //}

        //[Fact]
        //public async Task TestDeletItemFromOrder_Ok()
        //{
        //    // Arrange
        //    // Act
        //    var result = await _OrderRepo.RemoveItemFromOrder(1, 1, 1);
        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Id.Should().Be(1);
        //    result.TotalAmount.Should().Be(470);
        //}

        [Fact]
        public async Task TestDeletItemFromOrder_Error()
        {

            await Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _OrderRepo.RemoveItemFromOrder(1, 10, 1)
                );

        }

        [Fact]
        public async Task TestOrderCheckOut_withInvalidOrder()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _OrderRepo.OrderCheckOut(10)
                );
        }

        [Fact]
        public async Task TestOrderCheckOut_Ok()
        {
            var result = await _OrderRepo.OrderCheckOut(1);
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.TotalAmount.Should().Be(560);
        }
    }
}
