using CommerceApp.DTOs.Orders;

namespace CommerceApp.Contracts.Orders
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrdersByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
        Task<bool> CancelOrderAsync(int id);

    }
}