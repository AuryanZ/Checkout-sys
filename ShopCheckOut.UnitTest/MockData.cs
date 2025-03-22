using ShopCheckOut.API.Models;

namespace ShopCheckOut.UnitTest
{
    public class MockData
    {
        public MockData() { }

        private List<ProductsModel> _mockProducts = new List<ProductsModel>
                    {
                        new () { Id = 1, SKU = "SKU1", Name = "Product1", Category = "Category1", Price = 10.0m, PriceUnit = "kg" },
                        new () { Id = 2, SKU = "SKU2", Name = "Product2", Brand = "MockBrand", Category = "Category2", Price = 20.0m, PriceUnit = "item" },
                        new () { Id = 3, SKU = "SKU3", Name = "Product3", Brand = "Foo", Category = "Category1", Price = 30.0m, PriceUnit = "g" },
                        new () { Id = 4, SKU = "SKU4", Name = "Product4", Category = "Category1", Price = 40.0m, PriceUnit = "kg" },
                        new () { Id = 5, SKU = "SKU5", Name = "Product5", Category = "Category2", Price = 50.0m, PriceUnit = "item" },
                    };

        private List<OrdersModel> _mockOrders;

        public List<ProductsModel> GetMockProducts()
        {
            return _mockProducts;
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
                    TotalAmount = 90.0m
                    },
                    new() {Id = 2, CustomerId = "0009123", OrderDate = new DateTime(2025,3,21),
                    OrderItems= new List<OrderItems>{
                                new OrderItems() {Id =13, OrderId = 2, ProductId = 2, Product = _mockProducts.FirstOrDefault(p=>p.Id == 2), Quantity = 1},
                                new OrderItems() {Id =14, OrderId = 2, ProductId=3, Product = _mockProducts.FirstOrDefault(p=>p.Id == 3), Quantity = 4},
                                new OrderItems() {Id =15, OrderId = 2, ProductId = 5, Product = _mockProducts.FirstOrDefault(p=>p.Id == 5), Quantity = 1},
                    },
                    TotalAmount = 190.0m
                    }
                };
            return _mockOrders;
        }




    }
}
