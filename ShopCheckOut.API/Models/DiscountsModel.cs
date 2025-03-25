using System.ComponentModel.DataAnnotations;

namespace ShopCheckOut.API.Models
{
    public record class DiscoutRemainder
    {
        public int Price { get; set; }
        public int RemaindQuantity { get; set; }
        public DiscoutRemainder(int price, int remaindQuantity)
        {
            this.Price = price;
            this.RemaindQuantity = remaindQuantity;
        }
    }
    public abstract class DiscountsModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int MinQuantity { get; set; }
        public abstract DiscoutRemainder ApplyDiscount(int originalPrice, int quantity);
        public abstract int GetMinQuantity();
    }
}
