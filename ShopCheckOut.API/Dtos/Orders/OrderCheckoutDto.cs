namespace ShopCheckOut.API.Dtos.Orders
{
    public class OrderCheckoutDto
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<ItemUpdateDto> OrderItems { get; set; }
        public int TotalAmount { get; set; }
    }
}
