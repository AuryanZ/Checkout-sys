using System.ComponentModel.DataAnnotations;

namespace ShopCheckOut.API.Models
{
    public record class discoutRemainder
    {
        public int price { get; set; }
        public int remaindQuantity { get; set; }
        public discoutRemainder(int price, int remaindQuantity)
        {
            this.price = price;
            this.remaindQuantity = remaindQuantity;
        }
    }
    public abstract class DiscountsModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int minQuantity { get; set; }
        public abstract discoutRemainder ApplyDiscount(int originalPrice, int quantity);
    }
}
