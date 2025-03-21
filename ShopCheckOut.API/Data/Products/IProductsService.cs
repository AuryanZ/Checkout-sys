using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Products
{
    public interface IProductsService
    {
        Task<List<ProductsModel>> GetProducts();
        Task<List<ProductsModel>> GetProductsByCategory(string category);
        Task<ProductsModel> GetProductBySKU(string sku);
        Task<bool> AddProduct(ProductsModel product);
    }
}
