namespace ShopCheckOut.API.Models
{
    public class DiscountsModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int MinQuantity { get; set; }
        public List<DiscountTiersModel>? DiscountTiers { get; set; }
    }
}
