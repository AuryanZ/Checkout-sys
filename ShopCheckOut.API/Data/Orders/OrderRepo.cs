using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Orders
{
    public class OrderRepo : IOrderRepo
    {
        private readonly IDiscountRepo _discountRepo = new DiscountRepo();
        private readonly MockData Dataset = new MockData();

        public Task<OrdersModel> NewOrder(OrdersModel newOrder)
        {
            newOrder.Id = Dataset._mockOrders.Any() ? Dataset._mockOrders.Max(o => o.Id) + 1 : 1;

            Dataset._mockOrders.Add(newOrder);
            return newOrder == null
                ? throw new ArgumentNullException("Order cannot be null")
                : Task.FromResult(newOrder);
        }

        public async Task<OrdersModel> AddItemToOrder(int orderId, ProductsModel product, int quantity)
        {
            var order = Dataset._mockOrders.FirstOrDefault(o => o.Id == orderId)
                ?? throw new KeyNotFoundException($"Order with ID {orderId} not found.");
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
                    Id = order.OrderItems.Count == 0 ? order.OrderItems.Max(oi => oi.Id) + 1 : 1,
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

        public async Task<OrdersModel> RemoveItemFromOrder(int orderId, int productId, int quantityRemove)
        {
            var order = Dataset._mockOrders.FirstOrDefault(o => o.Id == orderId) ??
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            var orderItem = (order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId))
                ?? throw new KeyNotFoundException($"Order Item with ID {productId} not found.");

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
            var order = Dataset._mockOrders.FirstOrDefault(o => o.Id == orderId);

            return order ?? throw new KeyNotFoundException($"Order ID {orderId} not found"); // if validation not required

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
            var productAfterDiscout = await _discountRepo.PriceAfterDiscount(productsModel, quantity);
            return productAfterDiscout;
        }
    }
}
