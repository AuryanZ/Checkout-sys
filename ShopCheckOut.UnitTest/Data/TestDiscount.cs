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
            // Arrange
            var mockDiscounts = new List<DiscountsModel>
                {
                    new DiscountsModel { Id = 1, Name = "10% Off", IsActive = true, MinQuantity = 2, DiscountTiers = new List<DiscountTiersModel>
                        {
                            new DiscountTiersModel { Id = 1, DiscountId = 1, Threshold = 2, Percentage = 10 }
                        }
                    },
                    new DiscountsModel { Id = 2, Name = "Inactive Discount", IsActive = false }
                };
            // Act
            var activeDiscounts = await _discountService.GetAvailableDiscounts(); // Await the task
                                                                                  // Assert
            Assert.NotNull(activeDiscounts);
            Assert.Equal(3, activeDiscounts.Count);
        }

        [Fact]
        public async Task AddNewDiscount_ShouldAddNewDiscount()
        {
            // Arrange
            var newDiscount = new DiscountsModel
            {
                Id = 5,
                Name = "New Discount",
                IsActive = true,
                MinQuantity = 2,
                DiscountTiers = new List<DiscountTiersModel>
                    {
                        new DiscountTiersModel { Id = 5, DiscountId = 5, Threshold = 2, Percentage = 10 }
                    }
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
            var newDiscount = new DiscountsModel
            {
                Id = 5,
                Name = "New Discount",
                IsActive = true,
                MinQuantity = 2,
                DiscountTiers = new List<DiscountTiersModel>
                    {
                        new DiscountTiersModel { Id = 5, DiscountId = 5, Threshold = 2, FixedPrice = 50 }
                    }
            };
            // Act
            var result = await _discountService.AddNewDiscout(newDiscount, 1);
            // Assert
            Assert.NotNull(result);
            Assert.True(result);
            // Act
            var deleteResult = await _discountService.DeleteDiscount(1);
            // Assert
            Assert.NotNull(deleteResult);
            Assert.True(deleteResult);
            // Act
            var activeDiscounts = await _discountService.GetAvailableDiscounts();
            // Assert
            Assert.NotNull(activeDiscounts);
            Assert.Equal(3, activeDiscounts.Count);
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

        [Fact]
        public async Task TestPriceAfterDiscout_WhenMultiDiscountApplied()
        {
            // Arrange
            var product = new ProductsModel
            {
                Id = 3,
                SKU = "SKU3",
                Name = "Product3",
                Brand = "Foo",
                Category = "Category1",
                Price = 130,
                PriceUnit = "g"
            };
            var quantity = 3;
            // Act
            var priceAfterDiscount = await _discountService.PriceAfterDiscount(product, quantity);
            // Assert
            Assert.NotNull(priceAfterDiscount);
            Assert.Equal(210, priceAfterDiscount.priceAfterDiscount);
            Assert.Equal("Special Price", priceAfterDiscount.highesDiscount.Name);
        }
    }
}
