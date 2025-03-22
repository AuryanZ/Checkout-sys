namespace ShopCheckOut.API.Dtos.Orders
{
    public class AddItemRequest
    {
        public string ItemSKU { get; set; }
        public string Quantity { get; set; }
    }
}
