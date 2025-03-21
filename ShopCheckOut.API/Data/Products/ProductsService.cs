using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Products
{

    public class ProductsService : IProductsService
    {
        // Mocking the products list    
        private readonly List<ProductsModel> _mockProducts = new List<ProductsModel>
            {
                new () { Id = 1, SKU = "SKU1", Name = "Product1", Category = "Category1", Price = 10.0m, PriceUnit = "kg" },
                new () { Id = 2, SKU = "SKU2", Name = "Product2", Category = "Category2", Price = 20.0m, PriceUnit = "item" },
                new () { Id = 3, SKU = "SKU3", Name = "Product3", Category = "Category1", Price = 30.0m, PriceUnit = "g" },
                new () { Id = 4, SKU = "SKU4", Name = "Product4", Category = "Category1", Price = 40.0m, PriceUnit = "kg" },
                new () { Id = 5, SKU = "SKU5", Name = "Product5", Category = "Category2", Price = 50.0m, PriceUnit = "item" },
            };
        //public ProductsService(List<Products> mockProducts)
        public ProductsService()
        {
            //_mockProducts = mockProducts;
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
            try
            {
                var result = _mockProducts.FirstOrDefault(p => p.SKU == sku);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
