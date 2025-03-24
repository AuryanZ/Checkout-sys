using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Discounts
{
    public interface IDiscountService
    {
        Task<List<ProductDiscountModel>> GetAvailableDiscounts();
        Task<bool> AddNewDiscout(DiscountsModel discount, int productId);
        Task<bool> DeleteDiscount(int discountId);
        Task<PriceAfterDiscountReturn> PriceAfterDiscount(ProductsModel product, int quantity);
    }
}
