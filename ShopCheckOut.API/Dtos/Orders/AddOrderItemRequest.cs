namespace ShopCheckOut.API.Dtos.Orders
{
    public class AddItemRequest
    {
        public string ItemSku { get; set; }
        public string Quantity { get; set; }
    }
}
