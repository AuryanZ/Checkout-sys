using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest.Data
{
    public class TestProductFuncs
    {
        [Fact]
        public async Task GetProductsByCategory_ReturnsCorrectProducts()
        {
            // Arrange
            var service = new ProductsService();

            // Act
            var result = await service.GetProductsByCategory("Category1");
            var result2 = await service.GetProductsByCategory("Category2");

            // Assert
            Assert.Equal(3, result.Count);
            Assert.All<ProductsModel>(result, p => Assert.Equal("Category1", p.Category));

            Assert.Equal(3, result2.Count);
            Assert.All<ProductsModel>(result2, p => Assert.Equal("Category2", p.Category));
        }

        [Fact]
        public async Task GetProductBySKU_ReturnsCorrectProduct()
        {
            // Arrange
            var service = new ProductsService();
            // Act
            var result = await service.GetProductBySKU("SKU1");
            var result2 = await service.GetProductBySKU("SKU2");
            // Assert
            Assert.Equal("Product1", result.Name);
            Assert.Equal("Product2", result2.Name);
        }

        [Fact]
        public async Task GetProducts_ReturnsAllProducts()
        {
            // Arrange
            var service = new ProductsService();
            // Act
            var result = await service.GetProducts();
            // Assert
            Assert.Equal(6, result.Count);
        }

        [Fact]
        public async Task TestGetProductIdbySKU_Error()
        {
            var service = new ProductsService();
            await Assert.ThrowsAsync<Exception>(
                async () => await service.GetProductIdBySku("xyz")
                );
        }
    }
}
