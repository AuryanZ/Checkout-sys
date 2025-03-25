namespace ShopCheckOut.API.Dtos.Products
{
    public record class ProductReadDto()
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string PriceInfo { get; set; }
    }
}
