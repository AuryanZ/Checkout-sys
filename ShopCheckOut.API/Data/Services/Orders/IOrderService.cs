using ShopCheckOut.API.Dtos.Orders;

namespace ShopCheckOut.API.Data.Services.Orders
{
    public interface IOrderService
    {
        Task<OrderCreateDto> NewOrder(string? customerId);
        Task<OrderUpdateDto> AddItemToOrder(string orderId, AddItemRequest request);
        Task<OrderUpdateDto> RemoveItemFromOrder(string orderId, RemoveItemRequest request);
        Task<OrderCheckoutDto> OrderCheckOut(string orderId);
    }
}
