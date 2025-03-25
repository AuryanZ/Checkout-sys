using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Dtos.Discounts
{
    public enum DiscountTypes
    {
        Percentage,
        FixedPrice,
        BuyXGetY
    }

    public class AddDiscountRequest
    {
        public required string Name { get; set; }
        public required string ProductSKU { get; set; }
        public required bool IsActive { get; set; }
        public required int MinQuantity { get; set; }

        public required DiscountTypes DiscountType { get; set; }

        public int? Percentage { get; set; }
        public int? FixedPrice { get; set; }
        public int? FreeItem { get; set; }

        private static readonly Dictionary<DiscountTypes, Type> _DiscountType = new()
        {
            { DiscountTypes.Percentage, typeof(PercentageDiscount) },
            { DiscountTypes.FixedPrice, typeof(FixedPriceDiscount) },
            { DiscountTypes.BuyXGetY, typeof(BuyXGetYDiscount) }
        };

        public Type GetDiscountType()
        {
            return _DiscountType[DiscountType];
        }
    }
}
