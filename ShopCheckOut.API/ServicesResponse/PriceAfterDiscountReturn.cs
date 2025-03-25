public class PriceAfterDiscountReturn
{
    public int Price { get; set; }
    public int ItemSaved { get; set; }
    public string? DiscoutName { get; set; }

    public PriceAfterDiscountReturn(int price, int itemSaved, string? discoutName)
    {
        Price = price;
        ItemSaved = itemSaved;
        DiscoutName = discoutName;
    }
}
