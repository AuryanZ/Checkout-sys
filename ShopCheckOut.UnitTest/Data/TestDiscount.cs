using Moq;
using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data
{
    public class TestDiscountFuncs
    {
        private readonly Mock<IDiscountRepo> _discountRepo;
        public TestDiscountFuncs()
        {
            _discountRepo = new Mock<IDiscountRepo>();
        }
        [Fact]
        public async Task GetAvailableDiscounts_ShouldReturnActiveDiscounts()
        {
            // Arrange
            var activeDiscounts = new List<ProductDiscountModel>
                {
                    new ProductDiscountModel { ProductId = 1, DiscountId = 1 },
                    new ProductDiscountModel { ProductId = 2, DiscountId = 2 },
                    new ProductDiscountModel { ProductId = 3, DiscountId = 3 },
                    new ProductDiscountModel { ProductId = 4, DiscountId = 4 },
                    new ProductDiscountModel { ProductId = 5, DiscountId = 5 },
                    new ProductDiscountModel { ProductId = 6, DiscountId = 6 },
                    new ProductDiscountModel { ProductId = 7, DiscountId = 7 }
                };
            _discountRepo.Setup(service => service.GetAvailableDiscounts()).ReturnsAsync(activeDiscounts);

            // Act
            var result = await _discountRepo.Object.GetAvailableDiscounts();

            // Assert

            Assert.NotNull(result);
            Assert.Equal(7, result.Count);
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
                MinQuantity = 1
            };
            // Act
            await _discountRepo.Object.AddNewDiscout(newDiscount, 1);
            // Assert
            _discountRepo.Verify(service => service.AddNewDiscout(newDiscount, 1), Times.Once);

        }

        [Fact]
        public async Task DeleteDiscount_ShouldDeleteDiscount()
        {
            // Arrange
            var discountId = 1;
            // Act
            await _discountRepo.Object.DeleteDiscount(discountId);
            // Assert
            _discountRepo.Verify(service => service.DeleteDiscount(discountId), Times.Once);

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
            var discountRepo = new DiscountRepo();
            // Act
            await discountRepo.AddNewDiscout(newDiscount, 1);

            var activeDiscounts = await discountRepo.GetAvailableDiscounts();
            Assert.NotNull(activeDiscounts);
            Assert.Equal(8, activeDiscounts.Count);

            await discountRepo.DeleteDiscount(1);

            activeDiscounts = await discountRepo.GetAvailableDiscounts();
            Assert.NotNull(activeDiscounts);
            Assert.Equal(7, activeDiscounts.Count);
        }

        [Fact]
        public async Task TestPriceAfterDiscout_WhenNoDiscountApplied()
        {
            // Arrange
            var discountRepo = new DiscountRepo();
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
            var priceAfterDiscount = await discountRepo.PriceAfterDiscount(product, quantity);
            // Assert
            Assert.Equal(100, priceAfterDiscount.Price);
            Assert.Null(priceAfterDiscount.DiscoutName);
        }

        [Fact]
        public async Task TestPriceAfterDiscout_WhenDiscountApplied()
        {
            // Arrange
            var discountRepo = new DiscountRepo();
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
            var priceAfterDiscount = await discountRepo.PriceAfterDiscount(product, quantity);
            // Assert
            Assert.NotNull(priceAfterDiscount);
            Assert.Equal(180, priceAfterDiscount.Price);
            Assert.Equal("10% Off", priceAfterDiscount.DiscoutName);
            Assert.Equal(20, priceAfterDiscount.ItemSaved);
        }

        [Fact]
        public async Task TestPriceAfterDiscout_WhenTireDiscoutApplied()
        {
            // Arrange
            var discountRepo = new DiscountRepo();
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
            var priceAfterDiscount = await discountRepo.PriceAfterDiscount(product, quantity);
            // Assert
            Assert.NotNull(priceAfterDiscount);
            Assert.Equal(180, priceAfterDiscount.Price);
            Assert.Equal("Buy 10 Get 5 Free; Buy 5 Get 2 Free; ", priceAfterDiscount.DiscoutName);
            Assert.Equal(70, priceAfterDiscount.ItemSaved);
        }

    }
}
