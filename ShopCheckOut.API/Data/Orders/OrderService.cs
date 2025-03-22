using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Orders
{
    public class OrderService : IOrderService
    {
        private readonly List<OrdersModel> _mockOrders;

        public OrderService()
        {
            _mockOrders = new MockData().GetMockOrders();
        }

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
                    Product = product,
                    Quantity = quantity
                };

                order.OrderItems.Add(newOrderItem);
            }
            decimal _price = product.Price * quantity;
            order.TotalAmount += _price;

            return Task.FromResult(order);
        }

        public Task<OrdersModel> DeleteItemFromOrder(int orderId, int productId, int quantityRemove)
        {
            var order = _mockOrders.FirstOrDefault(o => o.Id == orderId) ??
                throw new Exception($"Order with ID {orderId} not found.");
            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} not found.");
            }

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId) ??
                throw new Exception($"Order Item with ID {productId} not found.");
            if (orderItem == null)
            {
                throw new Exception($"Order Item with ID {productId} not found.");
            }
            if (quantityRemove > 0)
            {
                int quantityDiff = orderItem.Quantity - quantityRemove;
                if (quantityDiff <= 0)
                {
                    order.OrderItems.Remove(orderItem);
                    order.TotalAmount -= orderItem.Product.Price * orderItem.Quantity;
                }
                else
                {
                    orderItem.Quantity = quantityDiff;
                    order.TotalAmount -= orderItem.Product.Price * quantityRemove;
                }
            }

            return Task.FromResult(order);
        }

        public Task<OrdersModel> OrderCheckOut(int orderId)
        {
            var order = _mockOrders.FirstOrDefault(o => o.Id == orderId) ??
                throw new Exception("Invilide Order Id");
            if (order == null)
            {
                throw new Exception("Invilide Order Id");
            }

            //return Task.FromResult(order); // if validation not required

            var orderItems = order.OrderItems;
            List<OrderItems> tempOrderItem = new List<OrderItems>();
            var amount = 0.0m;
            foreach (var item in orderItems)
            {
                amount += item.Product.Price * item.Quantity;
                tempOrderItem.Add(item);
            }

            if (amount != order.TotalAmount)
            {
                order.OrderItems = tempOrderItem;
                order.TotalAmount = amount;
                return Task.FromResult(order);
            }

            return Task.FromResult(order);

        }
    }
}
