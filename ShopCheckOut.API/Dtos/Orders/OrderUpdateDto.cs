namespace ShopCheckOut.API.Dtos.Orders
{
    public class OrderUpdateDto
    {
        public int Id { get; set; }
        public List<ItemUpdateDto> OrderItems { get; set; }
        public int TotalAmount { get; set; }
    }
}
