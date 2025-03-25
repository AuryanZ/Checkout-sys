using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data
{
    public class MockData
    {
        public List<ProductsModel> _mockProducts;

        public List<DiscountsModel> _mockDiscounts;

        public List<ProductDiscountModel> _mockProductDiscounts;

        public List<OrdersModel> _mockOrders;

        public MockData()
        {
            _mockProducts = new List<ProductsModel>
            {
                new () { Id = 1, Sku = "SKU1", Name = "Product1", Category = "Category1", Price = 100, PriceUnit = "kg" },
                new () { Id = 2, Sku = "SKU2", Name = "Product2", Brand = "MockBrand", Category = "Category2", Price = 120, PriceUnit = "item" },
                new () { Id = 3, Sku = "SKU3", Name = "Product3", Brand = "Foo", Category = "Category1", Price = 130, PriceUnit = "g" },
                new () { Id = 4, Sku = "SKU4", Name = "Product4", Category = "Category1", Price = 140, PriceUnit = "kg" },
                new () { Id = 5, Sku = "SKU5", Name = "Product5", Category = "Category2", Price = 150, PriceUnit = "item" },
                new () { Id = 6, Sku = "SKU6", Name = "Product6", Category = "Category2", Price = 10, PriceUnit = "item" },
            };
            _mockDiscounts = new List<DiscountsModel>{
                new PercentageDiscount { Id = 1, Name = "10% Off", IsActive = true, Percentage = 10, MinQuantity = 2 },
                new FixedPriceDiscount { Id = 2, Name = "Fixed $100", IsActive = true, FixedPrice = 100, MinQuantity = 1 },
                new BuyXGetYDiscount { Id = 3, Name = "Buy 2 Get 1 Free", IsActive = true, MinQuantity = 2, FreeItem = 1 },
                new PercentageDiscount { Id = 4, Name = "20% Off", IsActive = false, Percentage = 20, MinQuantity = 3 },
                new BuyXGetYDiscount { Id = 5, Name = "Buy 1 Get 1 Free", IsActive = true, MinQuantity = 1, FreeItem = 1 },
                new PercentageDiscount { Id = 6, Name = "Inactive Discount", IsActive = false, Percentage = 10, MinQuantity = 1 },
                new BuyXGetYDiscount { Id = 7, Name = "Buy 3 Get 1 Free", IsActive = true, MinQuantity = 3, FreeItem = 1 },
                new BuyXGetYDiscount { Id = 8, Name = "Buy 5 Get 2 Free", IsActive = true, MinQuantity = 5, FreeItem = 2 },
                new BuyXGetYDiscount { Id = 9,  Name = "Buy 10 Get 5 Free",  IsActive = true, MinQuantity = 10, FreeItem = 5 },
            };
            _mockProductDiscounts = new List<ProductDiscountModel>
            {
                new() { ProductId = 1, DiscountId = 1 },
                new() { ProductId = 2, DiscountId = 2 },
                new() { ProductId = 3, DiscountId = 3 },
                new() { ProductId = 4, DiscountId = 5 },
                new() { ProductId = 6, DiscountId = 7 },
                new() { ProductId = 6, DiscountId = 8 },
                new() { ProductId = 6, DiscountId = 9 },
            };
            _mockOrders = new List<OrdersModel>{
                new() {Id = 1, CustomerId = null, OrderDate= new DateTime(2025, 3, 5),
                OrderItems =new List<OrderItems>{
                            new OrderItems() {Id =10, OrderId = 1, ProductId = 1, Product = _mockProducts.FirstOrDefault(p=>p.Id == 1), Quantity = 3, DiscountName ="10% Off", Saved = 30 },
                            new OrderItems() {Id =11, OrderId = 1, ProductId=4, Product = _mockProducts.FirstOrDefault(p=>p.Id == 4), Quantity = 1},
                            new OrderItems() {Id =12, OrderId = 1, ProductId = 2, Product = _mockProducts.FirstOrDefault(p=>p.Id == 2), Quantity = 1},
                            },
                TotalAmount = 560,
                TotalSaved = 30
                },
                new() {Id = 2, CustomerId = "0009123", OrderDate = new DateTime(2025,3,21),
                OrderItems= new List<OrderItems>{
                            new OrderItems() {Id =13, OrderId = 2, ProductId = 2, Product = _mockProducts.FirstOrDefault(p=>p.Id == 2), Quantity = 1, DiscountName = "Fixed $100", Saved = 20},
                            new OrderItems() {Id =14, OrderId = 2, ProductId=3, Product = _mockProducts.FirstOrDefault(p=>p.Id == 3), Quantity = 4, DiscountName = "Buy 2 Get 1 Free", Saved =130},
                            new OrderItems() {Id =15, OrderId = 2, ProductId = 5, Product = _mockProducts.FirstOrDefault(p=>p.Id == 5), Quantity = 1},
                },
                TotalAmount = 640,
                TotalSaved = 150
                }
            };
        }

    }
}
