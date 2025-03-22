using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Products
{

    public class ProductsService : IProductsService
    {
        // Mocking the products list    
        private readonly List<ProductsModel> _mockProducts;
        //public ProductsService(List<Products> mockProducts)
        public ProductsService()
        {
            //_mockProducts = mockProducts;
            _mockProducts = new MockData().GetMockProducts();
        }

        public Task<bool> AddProduct(ProductsModel product)
        {
            try
            {
                _mockProducts.Add(product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Task.FromResult(true);
        }

        public Task<ProductsModel> GetProductBySKU(string sku)
        {

            var result = _mockProducts.FirstOrDefault(p => p.SKU == sku) ??
                throw new Exception("Invalid product SKU");

            return Task.FromResult(result);

        }

        public Task<string> GetProductIdBySku(string sku)
        {
            var result = _mockProducts.FirstOrDefault(p => p.SKU == sku) ??
                throw new Exception("Invalid product SKU");

            return Task.FromResult(result.Id.ToString());

        }

        public Task<List<ProductsModel>> GetProducts()
        {
            try
            {
                return Task.FromResult(_mockProducts);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<List<ProductsModel>> GetProductsByCategory(string category)
        {
            try
            {
                var result = _mockProducts.Where(p => p.Category == category).ToList();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
