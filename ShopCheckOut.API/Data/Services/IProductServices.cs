using ShopCheckOut.API.Dtos.Products;

namespace ShopCheckOut.API.Data.Services
{
    public interface IProductServices
    {
        Task<List<ProductReadDto>> GetProducts();
        Task<ProductReadDto> GetProductBySKU(string sku);
        Task<List<ProductReadDto>> GetProductsByCategory(string category);
    }
}
