using AutoMapper;
using Microsoft.Data.SqlClient;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Products;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Services.Products
{
    public class ProductServices(IProductsRepo productsRepo, IMapper mapper) : IProductServices
    {
        public async Task<ProductReadDto> GetProductBySKU(string sku)
        {
            try
            {
                var product = await productsRepo.GetProductBySKU(sku);
                var result = mapper.Map<ProductReadDto>(product);

                return result;
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database connection error", ex);
            }
            catch (TimeoutException ex)
            {
                throw new ApplicationException("Database timeout error", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<List<ProductReadDto>> GetProducts()
        {
            try
            {
                var products = await productsRepo.GetProducts()
                    ?? throw new KeyNotFoundException("No products found");
                var result = mapper.Map<List<ProductReadDto>>(products);
                return result;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database connection error", ex);
            }
            catch (TimeoutException ex)
            {
                throw new ApplicationException("Database timeout error", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<List<ProductReadDto>> GetProductsByCategory(string category)
        {
            try
            {
                var products = await productsRepo.GetProductsByCategory(category);
                if (products.Count == 0)
                {
                    throw new KeyNotFoundException($"No products found in category {category}");
                }
                var result = mapper.Map<List<ProductReadDto>>(products);
                return result;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database connection error", ex);
            }
            catch (TimeoutException ex)
            {
                throw new ApplicationException("Database timeout error", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<Task> AddProduct(ProductCreateDto product)
        {
            try
            {
                var newProduct = mapper.Map<ProductsModel>(product);
                await productsRepo.AddProduct(newProduct);
                return Task.CompletedTask;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database connection error", ex);
            }
            catch (TimeoutException ex)
            {
                throw new ApplicationException("Database timeout error", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
