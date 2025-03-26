using FluentAssertions;
using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data.Discounts
{
    public class TestDiscountFuncs
    {
        private readonly DiscountRepo _discountRepo = new DiscountRepo();

        [Fact]
        public async Task GetAvailableDiscounts_ShouldReturnActiveDiscounts()
        {
            // Arrange
            // Act
            var result = await _discountRepo.GetAvailableDiscounts();

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(7);
        }

        [Fact]
        public async Task AddNewDiscount_ShouldAddNewDiscount()
        {
            // Arrange
            var newDiscount = new PercentageDiscount
            {
                Id = 10,
                Name = "New Discount",
                IsActive = true,
                Percentage = 10,
                MinQuantity = 1
            };
            // Act
            await _discountRepo.AddNewDiscout(newDiscount, 1);
            var result = await _discountRepo.GetAvailableDiscounts();
            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(8);
        }

        [Fact]
        public async Task AddNewDiscount_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var newDiscount = new PercentageDiscount
            {
                Id = 7,
                Name = "New Discount",
                IsActive = true,
                Percentage = 10,
                MinQuantity = 1
            };
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _discountRepo.AddNewDiscout(newDiscount, 16));
        }

        [Fact]
        public async Task DeleteDiscount_ShouldDeleteDiscount()
        {
            // Arrange
            var discountId = 1;
            // Act
            await _discountRepo.DeleteDiscount(discountId);
            var result = await _discountRepo.GetAvailableDiscounts();
            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(6);
        }

        [Fact]
        public async Task Similate_Add_Delete_FinallyCheckAvalibleDiscout()
        {
            //Arrange
            var newDiscount = new PercentageDiscount
            {
                Id = 10,
                Name = "New Discount",
                IsActive = true,
                Percentage = 10,
                MinQuantity = 1
            };
            // Act
            await _discountRepo.AddNewDiscout(newDiscount, 1);

            var activeDiscounts = await _discountRepo.GetAvailableDiscounts();
            activeDiscounts.Should().NotBeNull();
            activeDiscounts.Count.Should().Be(8);

            await _discountRepo.DeleteDiscount(1);

            activeDiscounts = await _discountRepo.GetAvailableDiscounts();
            activeDiscounts.Should().NotBeNull();
            activeDiscounts.Count.Should().Be(7);
        }

        [Fact]
        public async Task TestPriceAfterDiscout_WhenNoDiscountApplied()
        {
            // Arrange
            var product = new ProductsModel
            {
                Id = 4,
                Sku = "SKU1",
                Name = "Product1",
                Category = "Category1",
                Price = 100,
                PriceUnit = "kg"
            };
            var quantity = 1;

            // Act
            var priceAfterDiscount = await _discountRepo.PriceAfterDiscount(product, quantity);
            // Assert
            priceAfterDiscount.Should().NotBeNull();
            priceAfterDiscount.Price.Should().Be(100);
            priceAfterDiscount.DiscoutName.Should().BeNull();
        }

        [Fact]
        public async Task TestPriceAfterDiscout_WhenDiscountApplied()
        {
            // Arrange
            var product = new ProductsModel
            {
                Id = 1,
                Sku = "SKU1",
                Name = "Product1",
                Category = "Category1",
                Price = 100,
                PriceUnit = "kg"
            };
            var quantity = 2;
            // Act
            var priceAfterDiscount = await _discountRepo.PriceAfterDiscount(product, quantity);
            // Assert
            priceAfterDiscount.Should().NotBeNull();
            priceAfterDiscount.Price.Should().Be(180);
            priceAfterDiscount.DiscoutName.Should().Be("10% Off");
            priceAfterDiscount.ItemSaved.Should().Be(20);
        }

        [Fact]
        public async Task TestPriceAfterDiscout_WhenTireDiscoutApplied()
        {
            // Arrange
            var product = new ProductsModel
            {
                Id = 6,
                Sku = "SKU6",
                Name = "Product6",
                Category = "Category2",
                Price = 10,
                PriceUnit = "item"
            };
            var quantity = 25;
            // Act
            var priceAfterDiscount = await _discountRepo.PriceAfterDiscount(product, quantity);
            // Assert
            priceAfterDiscount.Should().NotBeNull();
            priceAfterDiscount.Price.Should().Be(180);
            priceAfterDiscount.DiscoutName.Should().Be("Buy 10 Get 5 Free; Buy 5 Get 2 Free; ");
            priceAfterDiscount.ItemSaved.Should().Be(70);
        }

    }
}
