namespace ShopCheckOut.API.Dtos.Products
{
    public class ProductCreateDto
    {
        public required string SKU { get; set; }
        public required string Name { get; set; }
        public string? Brand { get; set; }
        public required string Category { get; set; }
        public required decimal Price { get; set; }
        public required string PriceUnit { get; set; }
    }
}
