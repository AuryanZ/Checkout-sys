namespace ShopCheckOut.API.Dtos.Orders
{
    public class RemoveItemRequest
    {
        public string ItemSku { get; set; }
        public string Quantity { get; set; }
    }
}
