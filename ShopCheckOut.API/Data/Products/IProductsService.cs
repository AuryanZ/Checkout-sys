namespace ShopCheckOut.API.Data.Products
{
    using ShopCheckOut.API.Models;
    public interface IProductsService
    {
        Task<List<Products>> GetProducts();
        Task<List<Products>> GetProductsByCategory(string category);
        Task<Products> GetProductBySKU(string sku);
        Task<bool> AddProduct(Products product);
    }
}
