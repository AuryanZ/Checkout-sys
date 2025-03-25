namespace ShopCheckOut.API.Models
{

    public class PercentageDiscount : DiscountsModel
    {
        public int Percentage { get; set; }
        public override DiscoutRemainder ApplyDiscount(int originalPrice, int quantity)
        {
            if (quantity >= MinQuantity)
            {
                return new DiscoutRemainder(originalPrice * (100 - Percentage) / 100 * quantity, 0);
            }

            return new DiscoutRemainder(originalPrice * quantity, quantity);
        }
        public override int GetMinQuantity()
        {
            return MinQuantity;
        }
    }
    public class FixedPriceDiscount : DiscountsModel
    {
        public int FixedPrice { get; set; }

        public override DiscoutRemainder ApplyDiscount(int originalPrice, int quantity)
        {
            if (quantity >= MinQuantity)
            {
                return new DiscoutRemainder(FixedPrice * quantity, 0);

            }
            return new DiscoutRemainder(originalPrice * quantity, quantity);
        }
        public override int GetMinQuantity()
        {
            return MinQuantity;
        }
    }

    public class BuyXGetYDiscount : DiscountsModel
    {
        public int FreeItem { get; set; }

        public override DiscoutRemainder ApplyDiscount(int originalPrice, int quantity)
        {
            int totalPaidItems = quantity - (quantity / (MinQuantity + FreeItem)) * FreeItem;
            int remaindQuantity = quantity % (MinQuantity + FreeItem);

            return new DiscoutRemainder(originalPrice * totalPaidItems, remaindQuantity);
        }
        public override int GetMinQuantity()
        {
            return MinQuantity + FreeItem;
        }
    }
}
