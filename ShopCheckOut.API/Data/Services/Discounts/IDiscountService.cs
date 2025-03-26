using ShopCheckOut.API.Dtos.Discounts;

namespace ShopCheckOut.API.Data.Services.Discounts
{
    public interface IDiscountService
    {
        Task<List<ProductDiscountDto>> GetDiscounts();
        Task<Task> AddNewDiscout(AddDiscountRequest request);
        Task DeleteDiscount(int discountId);
    }
}
