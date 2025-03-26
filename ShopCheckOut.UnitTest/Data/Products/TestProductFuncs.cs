using FluentAssertions;
using ShopCheckOut.API.Data.Products;

namespace ShopCheckOut.UnitTest.Data.Products
{
    public class TestProductFuncs
    {
        private readonly ProductsRepo _productsRepo = new ProductsRepo();
        [Fact]
        public async Task GetProductsByCategory_ReturnsCorrectProducts()
        {
            // Arrange
            // Act
            var result = await _productsRepo.GetProductsByCategory("Category1");

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(3);
            result.Should().OnlyContain(p => p.Category == "Category1");
        }

        [Fact]
        public async Task GetProductBySKU_ReturnsCorrectProduct()
        {
            // Arrange
            // Act
            var result = await _productsRepo.GetProductBySKU("SKU1");

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Product1");
        }

        [Fact]
        public async Task GetProducts_ReturnsAllProducts()
        {
            // Arrange
            // Act
            var result = await _productsRepo.GetProducts();

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(6);
        }

        [Fact]
        public async Task TestGetProductIdbySKU_Error()
        {
            // Arrange
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _productsRepo.GetProductIdBySku("xyz")
            );
        }
    }
}
