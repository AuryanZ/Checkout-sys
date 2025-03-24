using ShopCheckOut.API.Dtos.Products;

namespace ShopCheckOut.API.Dtos.Orders
{
    public class ItemUpdateDto
    {
        public int Quantity { get; set; }
        public int Saved { get; set; }
        public string? DiscountName { get; set; }
        public ProductReadDto Product { get; set; }
    }
}
