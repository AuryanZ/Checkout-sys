using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Orders
{
    public class OrderService : IOrderService
    {
        // Mocking the Order list    
        private readonly List<OrdersModel> _mockOrders = new List<OrdersModel>
            {
                new() {Id = 1, CustomerId = null, OrderDate= new DateTime(2025, 3, 5),
                OrderItems =new List<OrderItems>{
                            new OrderItems() {Id =10, OrderId = 1, ProductId = 1, Quantity = 3},
                            new OrderItems() {Id =11, OrderId = 1, ProductId=4, Quantity = 1},
                            new OrderItems() {Id =12, OrderId = 1, ProductId = 2, Quantity = 1},
                            },
                TotalAmount = 90.0m
                },
                new() {Id = 2, CustomerId = "0009123", OrderDate = new DateTime(2025,3,21),
                OrderItems= new List<OrderItems>{
                            new OrderItems() {Id =13, OrderId = 2, ProductId = 2, Quantity = 1},
                            new OrderItems() {Id =14, OrderId = 2, ProductId=3, Quantity = 4},
                            new OrderItems() {Id =15, OrderId = 2, ProductId = 5, Quantity = 1},
                },
                TotalAmount = 190.0m
                }
            };
        public Task<OrdersModel> NewOrder(string? customerId)
        {
            var newOrder = new OrdersModel
            {
                Id = _mockOrders.Any() ? _mockOrders.Max(o => o.Id) + 1 : 1,
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                OrderItems = new List<OrderItems>(),
                TotalAmount = 0
            };
            try
            {
                _mockOrders.Add(newOrder);
                return Task.FromResult(newOrder);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<OrdersModel> AddItemToOrder(int orderId, ProductsModel product, int quantity)
        {
            var order = _mockOrders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} not found.");
            }

            var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == product.Id);
            if (existingItem != null)
            {
                // If product exists, increase quantity
                existingItem.Quantity += quantity;
            }
            else
            {
                var newOrderItem = new OrderItems
                {
                    Id = order.OrderItems.Any() ? order.OrderItems.Max(oi => oi.Id) + 1 : 1,
                    OrderId = orderId,
                    ProductId = product.Id,
                    Quantity = quantity
                };

                order.OrderItems.Add(newOrderItem);
            }
            decimal _price = product.Price * quantity;
            order.TotalAmount += _price;

            return Task.FromResult(order);
        }
    }
}
