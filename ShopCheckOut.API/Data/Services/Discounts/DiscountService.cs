using AutoMapper;
using Microsoft.Data.SqlClient;
using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Services.Discounts
{
    public class DiscountService(
        IDiscountRepo discountRepo,
        IProductsRepo productsRepo,
        IMapper mapper) : IDiscountService
    {
        public async Task<Task> AddNewDiscout(AddDiscountRequest request)
        {
            try
            {
                var discoutType = request.GetDiscountType()
                    ?? throw new KeyNotFoundException("Discount Type is Invalid");
                var productId = await productsRepo.GetProductIdBySku(request.ProductSKU);
                int _productId = int.Parse(productId);
                var discount = (DiscountsModel)mapper.Map(request, request.GetType(), discoutType);

                await discountRepo.AddNewDiscout(discount, _productId);
                return Task.CompletedTask;
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

        public Task DeleteDiscount(int discountId)
        {
            try
            {
                return discountRepo.DeleteDiscount(discountId);
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

        public async Task<List<ProductDiscountDto>> GetDiscounts()
        {
            try
            {
                var discounts = await discountRepo.GetAvailableDiscounts();
                var result = mapper.Map<List<ProductDiscountDto>>(discounts);
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
    }
}
