namespace ShopCheckOut.API.Models
{

    public class PercentageDiscount : DiscountsModel
    {
        public int Percentage { get; set; }
        public override discoutRemainder ApplyDiscount(int originalPrice, int quantity)
        {
            if (quantity >= minQuantity)
            {
                return new discoutRemainder(originalPrice * (100 - Percentage) / 100 * quantity, 0);
            }

            return new discoutRemainder(originalPrice * quantity, quantity);
        }
    }
    public class FixedPriceDiscount : DiscountsModel
    {
        public int FixedPrice { get; set; }

        public override discoutRemainder ApplyDiscount(int originalPrice, int quantity)
        {
            if (quantity >= minQuantity)
            {
                return new discoutRemainder(FixedPrice * quantity, 0);

            }
            return new discoutRemainder(originalPrice * quantity, quantity);
        }
    }

    public class BuyXGetYDiscount : DiscountsModel
    {
        public int FreeItem { get; set; }

        public override discoutRemainder ApplyDiscount(int originalPrice, int quantity)
        {
            int totalPaidItems = quantity - (quantity / (minQuantity + FreeItem)) * FreeItem;
            int remaindQuantity = quantity % (minQuantity + FreeItem);

            return new discoutRemainder(originalPrice * totalPaidItems, remaindQuantity);
        }
    }
}
