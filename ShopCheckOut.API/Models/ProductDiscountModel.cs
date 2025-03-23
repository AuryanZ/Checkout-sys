namespace ShopCheckOut.API.Models
{
    public class ProductDiscountModel
    {
        public int ProductId { get; set; }
        public ProductsModel Product { get; set; }

        public int DiscountId { get; set; }
        public DiscountsModel Discount { get; set; }
    }
}
