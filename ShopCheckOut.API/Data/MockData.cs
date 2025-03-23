using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data
{
    public class MockData
    {
        public MockData() { }

        private List<ProductsModel> _mockProducts = new List<ProductsModel>
                    {
                        new () { Id = 1, SKU = "SKU1", Name = "Product1", Category = "Category1", Price = 100, PriceUnit = "kg" },
                        new () { Id = 2, SKU = "SKU2", Name = "Product2", Brand = "MockBrand", Category = "Category2", Price = 120, PriceUnit = "item" },
                        new () { Id = 3, SKU = "SKU3", Name = "Product3", Brand = "Foo", Category = "Category1", Price = 130, PriceUnit = "g" },
                        new () { Id = 4, SKU = "SKU4", Name = "Product4", Category = "Category1", Price = 140, PriceUnit = "kg" },
                        new () { Id = 5, SKU = "SKU5", Name = "Product5", Category = "Category2", Price = 150, PriceUnit = "item" },
                    };

        private List<DiscountsModel> _mockDiscounts = new List<DiscountsModel>
        {
            new() { Id = 1, Type = "Percentage", Name = "10% Off", IsActive = true, MinQuantity = 2,
                DiscountTiers = new List<DiscountTiersModel>
                {
                    new() { Id = 1, DiscountId = 1, Threshold = 2, Percentage = 90 }
                }
            },
            new() { Id = 2, Type = "Fixed Price", Name = "Special Price", IsActive = true, MinQuantity = 3,
                DiscountTiers = new List<DiscountTiersModel>
                {
                    new() { Id = 2, DiscountId = 2, Threshold = 3, FixedPrice = 9000 }
                }
            },
            new() { Id = 3, Type = "Free Item", Name = "Buy 2 Get 1 Free", IsActive = true, MinQuantity = 2,
                DiscountTiers = new List<DiscountTiersModel>
                {
                    new() { Id = 3, DiscountId = 3, Threshold = 3, FreeItem = 1 }
                }
            },
             new() { Id = 4, Type = "Free Item", Name = "Buy 1 Get 1 Free", IsActive = false, MinQuantity = 1,
                DiscountTiers = new List<DiscountTiersModel>
                {
                    new() { Id = 4, DiscountId = 4, Threshold = 2, FreeItem = 1 }
                }
            },
              new() { Id = 5, Type = "Fixed Price", Name = "Special Price", IsActive = true, MinQuantity = 2,
                DiscountTiers = new List<DiscountTiersModel>
                {
                    new() { Id = 5, DiscountId = 5, Threshold = 2, FixedPrice = 80 }
                }
            },
        };

        private List<ProductDiscountModel> _mockProductDiscounts = new List<ProductDiscountModel>
        {
            new() { ProductId = 1, DiscountId = 1 },
            new() { ProductId = 2, DiscountId = 2 },
            new() { ProductId = 3, DiscountId = 3 },
            new() { ProductId = 3, DiscountId = 5 }
        };

        private List<OrdersModel> _mockOrders;

        public List<ProductsModel> GetMockProducts()
        {
            return _mockProducts;
        }

        public List<DiscountsModel> GetMockDiscounts()
        {
            return _mockDiscounts;
        }

        public List<ProductDiscountModel> GetMockProductDiscounts()
        {
            return _mockProductDiscounts;
        }
        public List<OrdersModel> GetMockOrders()
        {
            _mockOrders = new List<OrdersModel>
                {
                    new() {Id = 1, CustomerId = null, OrderDate= new DateTime(2025, 3, 5),
                    OrderItems =new List<OrderItems>{
                                new OrderItems() {Id =10, OrderId = 1, ProductId = 1, Product = _mockProducts.FirstOrDefault(p=>p.Id == 1), Quantity = 3},
                                new OrderItems() {Id =11, OrderId = 1, ProductId=4, Product = _mockProducts.FirstOrDefault(p=>p.Id == 4), Quantity = 1},
                                new OrderItems() {Id =12, OrderId = 1, ProductId = 2, Product = _mockProducts.FirstOrDefault(p=>p.Id == 2), Quantity = 1},
                                },
                    TotalAmount = 560
                    },
                    new() {Id = 2, CustomerId = "0009123", OrderDate = new DateTime(2025,3,21),
                    OrderItems= new List<OrderItems>{
                                new OrderItems() {Id =13, OrderId = 2, ProductId = 2, Product = _mockProducts.FirstOrDefault(p=>p.Id == 2), Quantity = 1},
                                new OrderItems() {Id =14, OrderId = 2, ProductId=3, Product = _mockProducts.FirstOrDefault(p=>p.Id == 3), Quantity = 4},
                                new OrderItems() {Id =15, OrderId = 2, ProductId = 5, Product = _mockProducts.FirstOrDefault(p=>p.Id == 5), Quantity = 1},
                    },
                    TotalAmount = 790
                    }
                };
            return _mockOrders;
        }

    }
}
