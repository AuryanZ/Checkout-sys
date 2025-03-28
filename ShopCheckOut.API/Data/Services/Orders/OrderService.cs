using AutoMapper;
using Microsoft.Data.SqlClient;
using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Data.Orders;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Orders;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Services.Orders
{
    public class OrderService(
        IOrderRepo orderRepo,
        IProductsRepo productsRepo,
        IDiscountRepo discountRepo,
        IMapper mapper) : IOrderService
    {
        public async Task<OrderUpdateDto> AddItemToOrder(string orderId, AddItemRequest request)
        {
            if (request.Quantity == null || request.ItemSku == null)
            {
                throw new ArgumentNullException("Request data missing");
            }
            if (!int.TryParse(orderId, out var _orderId))
            {
                throw new ArgumentException("Invalid order ID format");
            }
            if (!int.TryParse(request.Quantity, out var _quantity))
            {
                throw new ArgumentException("Invalid quantity format");
            }
            try
            {
                var product = await productsRepo.GetProductBySKU(request.ItemSku);
                //var order = await orderRepo.AddItemToOrder(_orderId, product, _quantity);
                //return mapper.Map<OrderUpdateDto>(order);
                var order = await orderRepo.getOrderbyId(_orderId);
                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == product.Id);
                if (existingItem != null)
                {
                    var newOrderItem = new OrderItems
                    {
                        OrderId = _orderId,
                        ProductId = product.Id,
                        Product = product,
                        Quantity = _quantity
                    };
                    order = await orderRepo.AddOrderItem(newOrderItem, _orderId, true);
                    existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == product.Id);
                }
                else // If product does not exist, add new order item
                {
                    var newOrderItem = new OrderItems
                    {
                        OrderId = _orderId,
                        ProductId = product.Id,
                        Product = product,
                        Quantity = _quantity
                    };

                    order = await orderRepo.AddOrderItem(newOrderItem, _orderId, false);
                    existingItem = newOrderItem;
                }
                try
                {
                    var priceAfterDisc = await GetDiscountedPrice(existingItem);
                    existingItem.Saved = priceAfterDisc.ItemSaved;
                    existingItem.DiscountName = priceAfterDisc.DiscoutName;

                    order.TotalAmount += priceAfterDisc.Price;
                    order.TotalSaved += priceAfterDisc.ItemSaved;
                    var result = mapper.Map<OrderUpdateDto>(order);
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database connection error", ex);
            }
            catch (TimeoutException ex)
            {
                throw new ApplicationException("Database timeout error", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<OrderCreateDto> NewOrder(string? customerId)
        {
            try
            {
                var newOrder = new OrdersModel
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.Now,
                    OrderItems = [],
                    TotalAmount = 0
                };

                var result = await orderRepo.NewOrder(newOrder);
                return mapper.Map<OrderCreateDto>(result);
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database connection error", ex);
            }
            catch (TimeoutException ex)
            {
                throw new ApplicationException("Database timeout error", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<OrderCheckoutDto> OrderCheckOut(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ArgumentNullException("Order ID is missing");
            }
            if (!int.TryParse(orderId, out var _orderId))
            {
                throw new ArgumentException("Invalid order ID format");
            }
            try
            {
                var order = await orderRepo.OrderCheckOut(_orderId);
                return mapper.Map<OrderCheckoutDto>(order);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database connection error", ex);
            }
            catch (TimeoutException ex)
            {
                throw new ApplicationException("Database timeout error", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<OrderUpdateDto> RemoveItemFromOrder(string orderId, RemoveItemRequest request)
        {
            if (request.Quantity == null || request.ItemSku == null)
            {
                throw new ArgumentNullException("Request data missing.");
            }
            if (!int.TryParse(orderId, out var _orderId))
            {
                throw new ArgumentException("Invalid order ID format");
            }
            if (!int.TryParse(request.Quantity, out var _quantity))
            {
                throw new ArgumentException("Invalid quantity format");
            }
            try
            {
                var product = await productsRepo.GetProductBySKU(request.ItemSku);
                var order = await orderRepo.RemoveItemFromOrder(_orderId, product.Id, _quantity);
                return mapper.Map<OrderUpdateDto>(order);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database connection error", ex);
            }
            catch (TimeoutException ex)
            {
                throw new ApplicationException("Database timeout error", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        private async Task<PriceAfterDiscountReturn> GetDiscountedPrice(OrderItems order)
        {
            ProductsModel productsModel = order.Product;
            int quantity = order.Quantity;
            var productAfterDiscout = await discountRepo.PriceAfterDiscount(productsModel, quantity);
            return productAfterDiscout;
        }

    }

}
