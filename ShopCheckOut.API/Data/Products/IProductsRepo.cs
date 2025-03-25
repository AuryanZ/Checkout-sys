using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Products
{
    public interface IProductsRepo
    {
        Task<List<ProductsModel>> GetProducts();
        Task<List<ProductsModel>> GetProductsByCategory(string category);
        Task<ProductsModel> GetProductBySKU(string sku);

        Task<string> GetProductIdBySku(string sku);
        Task AddProduct(ProductsModel product);

    }
}
