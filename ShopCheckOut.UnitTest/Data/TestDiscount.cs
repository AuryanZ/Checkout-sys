using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data
{
    public class TestDiscountFuncs
    {
        private readonly DiscountService _discountService;
        public TestDiscountFuncs()
        {
            _discountService = new DiscountService();
        }
        [Fact]
        public async Task GetAvailableDiscounts_ShouldReturnActiveDiscounts()
        {
            // Act
            var activeDiscounts = await _discountService.GetAvailableDiscounts(); // Await the task
                                                                                  // Assert
            Assert.NotNull(activeDiscounts);
            Assert.Equal(4, activeDiscounts.Count);
        }

        [Fact]
        public async Task AddNewDiscount_ShouldAddNewDiscount()
        {
            // Arrange
            var newDiscount = new PercentageDiscount
            {
                Id = 7,
                Name = "New Discount",
                IsActive = true,
                Percentage = 10,
                minQuantity = 1
            };
            // Act
            var result = await _discountService.AddNewDiscout(newDiscount, 1);
            // Assert
            Assert.NotNull(result);
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteDiscount_ShouldDeleteDiscount()
        {
            // Arrange
            var discountId = 1;
            // Act
            var result = await _discountService.DeleteDiscount(discountId);
            // Assert
            Assert.NotNull(result);
            Assert.True(result);
        }

        [Fact]
        public async Task Similate_Add_Delete_FinallyCheckAvalibleDiscout()
        {
            // Arrange
            var newDiscount = new PercentageDiscount
            {
                Id = 7,
                Name = "New Discount",
                IsActive = true,
                Percentage = 10,
                minQuantity = 1
            };
            // Act
            var result = await _discountService.AddNewDiscout(newDiscount, 1);
            Assert.NotNull(result);
            Assert.True(result);

            var activeDiscounts = await _discountService.GetAvailableDiscounts();
            Assert.NotNull(activeDiscounts);
            Assert.Equal(5, activeDiscounts.Count);

            var deleteResult = await _discountService.DeleteDiscount(1);
            Assert.NotNull(deleteResult);
            Assert.True(deleteResult);

            activeDiscounts = await _discountService.GetAvailableDiscounts();
            Assert.NotNull(activeDiscounts);
            Assert.Equal(4, activeDiscounts.Count);
        }

        [Fact]
        public async Task TestPriceAfterDiscout_WhenNoDiscountApplied()
        {
            // Arrange
            var product = new ProductsModel
            {
                Id = 4,
                SKU = "SKU1",
                Name = "Product1",
                Category = "Category1",
                Price = 100,
                PriceUnit = "kg"
            };
            var quantity = 1;
            // Act
            var priceAfterDiscount = await _discountService.PriceAfterDiscount(product, quantity);
            // Assert
            Assert.Equal(100, priceAfterDiscount.priceAfterDiscount);
            Assert.Null(priceAfterDiscount.highesDiscount);
        }

        [Fact]
        public async Task TestPriceAfterDiscout_WhenDiscountApplied()
        {
            // Arrange
            var product = new ProductsModel
            {
                Id = 1,
                SKU = "SKU1",
                Name = "Product1",
                Category = "Category1",
                Price = 100,
                PriceUnit = "kg"
            };
            var quantity = 2;
            // Act
            var priceAfterDiscount = await _discountService.PriceAfterDiscount(product, quantity);
            // Assert
            Assert.NotNull(priceAfterDiscount);
            Assert.Equal(180, priceAfterDiscount.priceAfterDiscount);
            Assert.Equal("10% Off", priceAfterDiscount.highesDiscount.Name);
        }

    }
}
