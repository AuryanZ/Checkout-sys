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

        public Task<OrdersModel> getOrderbyId(int orderId)
        {
            var order = Dataset._mockOrders.FirstOrDefault(o => o.Id == orderId);
            return order == null
                ? throw new KeyNotFoundException($"Order with ID {orderId} not found.")
                : Task.FromResult(order);
        }

        public async Task<OrdersModel> RemoveItemFromOrder(int orderId, int productId, int quantityRemove)
        {
            var order = Dataset._mockOrders.FirstOrDefault(o => o.Id == orderId) ??
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            var orderItem = (order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId))
                ?? throw new KeyNotFoundException($"Order Item with ID {productId} not found.");

            //if (quantityRemove > 0)
            //{
            //    int quantityDiff = orderItem.Quantity - quantityRemove;
            //    if (quantityDiff <= 0) // If quantity to remove is greater than or equal to existing quantity, remove item
            //    {
            //        order.OrderItems.Remove(orderItem);
            //        var originalPrice = orderItem.Product.Price * orderItem.Quantity;
            //        order.TotalAmount -= originalPrice - orderItem.Saved;
            //        order.TotalSaved -= orderItem.Saved;
            //    }
            //    else
            //    {
            //        var originalItemAmount = (orderItem.Product.Price * orderItem.Quantity) - orderItem.Saved;
            //        var originalSaved = orderItem.Saved;

            //        orderItem.Quantity = quantityDiff;
            //        try
            //        {
            //            var priceAfterDisc = await GetDiscountedPrice(orderItem);

            //            orderItem.Saved = priceAfterDisc.ItemSaved;
            //            orderItem.DiscountName = priceAfterDisc.DiscoutName;

            //            order.TotalAmount -= originalItemAmount - priceAfterDisc.Price;
            //            order.TotalSaved -= originalSaved - priceAfterDisc.ItemSaved;
            //        }
            //        catch (Exception ex)
            //        {
            //            throw new Exception(ex.Message);
            //        }
            //    }
            //}

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

        public Task<OrdersModel> AddOrderItem(OrderItems orderItem, int orderid, bool isExist)
        {
            var order = Dataset._mockOrders.FirstOrDefault(o => o.Id == orderid);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderid} not found.");
            }
            if (isExist)
            {
                var existingOrderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == orderItem.ProductId);
                if (existingOrderItem != null)
                {
                    var originalItemAmount =
                        (existingOrderItem.Product.Price
                        * existingOrderItem.Quantity)
                        - existingOrderItem.Saved;
                    var originalSaved = existingOrderItem.Saved;

                    existingOrderItem.Quantity += orderItem.Quantity;
                    order.TotalAmount -= originalItemAmount;
                    order.TotalSaved -= originalSaved;
                }
            }
            else
            {
                orderItem.Id = order.OrderItems.Any() ? order.OrderItems.Max(oi => oi.Id) + 1 : 1;
                order.OrderItems.Add(orderItem);
            }




            return Task.FromResult(order);
        }

    }
}
