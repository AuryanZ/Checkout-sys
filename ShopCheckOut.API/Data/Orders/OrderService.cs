using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Orders
{

    public class OrderService : IOrderService
    {
        private readonly List<OrdersModel> _mockOrders;
        private readonly IDiscountService _discountService;

        public OrderService(IDiscountService discountService)
        {
            _mockOrders = new MockData().GetMockOrders();
            _discountService = discountService;
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

        public async Task<OrdersModel> AddItemToOrder(int orderId, ProductsModel product, int quantity)
        {
            var order = _mockOrders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} not found.");
            }

            var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == product.Id);
            if (existingItem != null)
            {
                var originalItemAmount = (existingItem.Product.Price * existingItem.Quantity) - existingItem.Saved;
                var originalSaved = existingItem.Saved;
                // If product exists, increase quantity
                existingItem.Quantity += quantity;
                order.TotalAmount -= originalItemAmount;
                order.TotalSaved -= originalSaved;
            }
            else // If product does not exist, add new order item
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
                existingItem = newOrderItem;
            }
            try
            {
                var priceAfterDisc = await GetDiscountedPrice(existingItem);
                existingItem.Saved = priceAfterDisc.ItemSaved;
                existingItem.DiscountName = priceAfterDisc.DiscoutName;

                order.TotalAmount += priceAfterDisc.Price;
                order.TotalSaved += priceAfterDisc.ItemSaved;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return order;
        }

        public async Task<OrdersModel> DeleteItemFromOrder(int orderId, int productId, int quantityRemove)
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
                if (quantityDiff <= 0) // If quantity to remove is greater than or equal to existing quantity, remove item
                {
                    order.OrderItems.Remove(orderItem);
                    var originalPrice = orderItem.Product.Price * orderItem.Quantity;
                    order.TotalAmount -= originalPrice - orderItem.Saved;
                    order.TotalSaved -= orderItem.Saved;
                }
                else
                {
                    var originalItemAmount = (orderItem.Product.Price * orderItem.Quantity) - orderItem.Saved;
                    var originalSaved = orderItem.Saved;

                    orderItem.Quantity = quantityDiff;
                    try
                    {
                        var priceAfterDisc = await GetDiscountedPrice(orderItem);

                        orderItem.Saved = priceAfterDisc.ItemSaved;
                        orderItem.DiscountName = priceAfterDisc.DiscoutName;

                        order.TotalAmount -= originalItemAmount - priceAfterDisc.Price;
                        order.TotalSaved -= originalSaved - priceAfterDisc.ItemSaved;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

            return order;
        }

        public async Task<OrdersModel> OrderCheckOut(int orderId)
        {
            var order = _mockOrders.FirstOrDefault(o => o.Id == orderId) ??
                throw new Exception("Invilide Order Id");
            if (order == null)
            {
                throw new Exception("Invilide Order Id");
            }

            return order; // if validation not required

            //var orderItems = order.OrderItems;
            //List<OrderItems> tempOrderItem = new List<OrderItems>();
            //var amount = 0;
            //foreach (var item in orderItems)
            //{
            //    amount += item.Product.Price * item.Quantity;
            //    tempOrderItem.Add(item);
            //}

            //if (amount != order.TotalAmount)
            //{
            //    order.OrderItems = tempOrderItem;
            //    order.TotalAmount = amount;
            //}

            //return order;
        }

        private async Task<PriceAfterDiscountReturn> GetDiscountedPrice(OrderItems order)
        {
            ProductsModel productsModel = order.Product;
            int quantity = order.Quantity;
            var productAfterDiscout = await _discountService.PriceAfterDiscount(productsModel, quantity);
            return productAfterDiscout;
        }
    }
}
