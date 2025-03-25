using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Products;

namespace ShopCheckOut.API.Data.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IProductsRepo _productsRepo;
        public ProductServices(IProductsRepo productsRepo)
        {
            _productsRepo = productsRepo;
        }
        public Task<ProductReadDto> GetProductBySKU(string sku)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductReadDto>> GetProducts()
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductReadDto>> GetProductsByCategory(string category)
        {
            throw new NotImplementedException();
        }
    }
}
