using ShopCheckOut.API.Models;
namespace ShopCheckOut.API.Data.Orders
{
    public interface IOrderRepo
    {
        Task<OrdersModel> NewOrder(OrdersModel? newOrder);

        Task<OrdersModel> AddItemToOrder(int orderId, ProductsModel product, int quantity);

        Task<OrdersModel> RemoveItemFromOrder(int orderId, int productId, int quantityRemove);

        Task<OrdersModel> OrderCheckOut(int orderId);
    }
}