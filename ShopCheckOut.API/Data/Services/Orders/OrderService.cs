using AutoMapper;
using Microsoft.Data.SqlClient;
using ShopCheckOut.API.Data.Orders;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Orders;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Data.Services.Orders
{
    public class OrderService(IOrderRepo orderRepo, IProductsRepo productsRepo, IMapper mapper) : IOrderService
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
                var order = await orderRepo.AddItemToOrder(_orderId, product, _quantity);
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
    }

}
