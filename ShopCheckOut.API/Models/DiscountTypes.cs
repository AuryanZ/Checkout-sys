namespace ShopCheckOut.API.Models
{
    public class PercentageDiscount : DiscountsModel
    {
        public int Percentage { get; set; }
        public int minQuantity { get; set; }
        public override int ApplyDiscount(int originalPrice, int quantity)
        {
            if (quantity >= minQuantity)
            {
                return originalPrice * (100 - Percentage) / 100 * quantity;
            }
            return originalPrice * quantity;
        }
    }
    public class FixedPriceDiscount : DiscountsModel
    {
        public int FixedPrice { get; set; }
        public int minQuantity { get; set; }

        public override int ApplyDiscount(int originalPrice, int quantity)
        {
            if (quantity >= minQuantity)
            {
                return FixedPrice * quantity;
            }
            return originalPrice * quantity;
        }
    }

    public class BuyXGetYDiscount : DiscountsModel
    {
        public int BuyQuantity { get; set; }
        public int FreeItem { get; set; }

        public override int ApplyDiscount(int originalPrice, int quantity)
        {
            int totalPaidItems = quantity - (quantity / (BuyQuantity + FreeItem)) * FreeItem;
            return originalPrice * totalPaidItems / quantity;
        }
    }
}
