using Moq;
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
            var mockRepo = new Mock<IProductsRepo>();
            mockRepo.Setup(repo => repo.GetProductsByCategory("Category1"))
                    .ReturnsAsync(new List<ProductsModel>
                    {
                        new ProductsModel { Name = "Product1", Category = "Category1" },
                        new ProductsModel { Name = "Product2", Category = "Category1" },
                        new ProductsModel { Name = "Product3", Category = "Category1" }
                    });
            mockRepo.Setup(repo => repo.GetProductsByCategory("Category2"))
                    .ReturnsAsync(new List<ProductsModel>
                    {
                        new ProductsModel { Name = "Product4", Category = "Category2" },
                        new ProductsModel { Name = "Product5", Category = "Category2" },
                        new ProductsModel { Name = "Product6", Category = "Category2" }
                    });

            var service = mockRepo.Object;

            // Act
            var result = await service.GetProductsByCategory("Category1");
            var result2 = await service.GetProductsByCategory("Category2");

            // Assert
            Assert.Equal(3, result.Count);
            Assert.All(result, p => Assert.Equal("Category1", p.Category));

            Assert.Equal(3, result2.Count);
            Assert.All(result2, p => Assert.Equal("Category2", p.Category));
        }

        [Fact]
        public async Task GetProductBySKU_ReturnsCorrectProduct()
        {
            // Arrange
            var mockRepo = new Mock<IProductsRepo>();
            mockRepo.Setup(repo => repo.GetProductBySKU("SKU1"))
                    .ReturnsAsync(new ProductsModel { Sku = "SKU1", Name = "Product1" });
            mockRepo.Setup(repo => repo.GetProductBySKU("SKU2"))
                    .ReturnsAsync(new ProductsModel { Sku = "SKU2", Name = "Product2" });

            var service = mockRepo.Object;

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
            var mockRepo = new Mock<IProductsRepo>();
            mockRepo.Setup(repo => repo.GetProducts())
                    .ReturnsAsync(new List<ProductsModel>
                    {
                        new ProductsModel { Name = "Product1" },
                        new ProductsModel { Name = "Product2" },
                        new ProductsModel { Name = "Product3" },
                        new ProductsModel { Name = "Product4" },
                        new ProductsModel { Name = "Product5" },
                        new ProductsModel { Name = "Product6" }
                    });

            var service = mockRepo.Object;

            // Act
            var result = await service.GetProducts();

            // Assert
            Assert.Equal(6, result.Count);
        }

        [Fact]
        public async Task TestGetProductIdbySKU_Error()
        {
            // Arrange
            var mockRepo = new Mock<IProductsRepo>();
            mockRepo.Setup(repo => repo.GetProductIdBySku("xyz"))
                    .ThrowsAsync(new KeyNotFoundException());

            var service = mockRepo.Object;

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await service.GetProductIdBySku("xyz")
            );
        }
    }
}
