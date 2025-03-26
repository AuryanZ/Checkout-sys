using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Dtos.Discounts
{
    public class ProductDiscountDto
    {
        public ProductsModel Product { get; set; }
        public DiscountsModel Discount { get; set; }
    }
}
