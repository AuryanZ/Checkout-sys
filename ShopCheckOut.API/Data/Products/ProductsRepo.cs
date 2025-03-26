using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Products
{

    public class ProductsRepo : IProductsRepo
    {
        // Mocking the products list    
        private readonly MockData _dataset = new MockData();

        public Task AddProduct(ProductsModel product)
        {
            _dataset._mockProducts.Add(product);

            return Task.CompletedTask;
        }

        public Task<ProductsModel> GetProductBySKU(string sku)
        {
            var result = _dataset._mockProducts.FirstOrDefault(p => p.Sku == sku);
            return result == null
                ? throw new KeyNotFoundException($"Product {sku} not found")
                : Task.FromResult(result);
        }

        public Task<string> GetProductIdBySku(string sku)
        {
            var result = _dataset._mockProducts.FirstOrDefault(p => p.Sku == sku);
            return result == null
                ? throw new KeyNotFoundException($"Product {sku} not found")
                : Task.FromResult(result.Id.ToString());
        }

        public Task<List<ProductsModel>> GetProducts()
        {
            return Task.FromResult(_dataset._mockProducts);
        }

        public Task<List<ProductsModel>> GetProductsByCategory(string category)
        {
            var result = _dataset._mockProducts.Where(p => p.Category == category).ToList();
            return Task.FromResult(result);

        }
    }
}
