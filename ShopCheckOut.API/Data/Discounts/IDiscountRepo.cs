using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Discounts
{
    public interface IDiscountRepo
    {
        Task<List<ProductDiscountModel>> GetAvailableDiscounts();
        Task AddNewDiscout(DiscountsModel discount, int productId);
        Task DeleteDiscount(int discountId);
        Task<PriceAfterDiscountReturn> PriceAfterDiscount(ProductsModel product, int quantity);
    }
}
