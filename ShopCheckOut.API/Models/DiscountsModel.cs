namespace ShopCheckOut.API.Models
{
    public abstract class DiscountsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public abstract int ApplyDiscount(int originalPrice, int quantity);
    }
}
