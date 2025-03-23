namespace ShopCheckOut.API.Models
{
    public class DiscountTiersModel
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public int Threshold { get; set; }
        public int? Percentage { get; set; }
        public int? FixedPrice { get; set; }
        public int? FreeItem { get; set; }
    }
}
