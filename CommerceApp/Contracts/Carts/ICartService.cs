using CommerceApp.DTOs.Cart;

namespace CommerceApp.Contracts.Carts
{
    public interface ICartService
    {
        Task<IEnumerable<CartDto>> GetAllCarts();
        Task<CartDto?> GetCartById(int id);
        Task<CartDto> AddItem(int cartId, int productId, int quantity);
        Task<CartDto?> UpdateItemQuantity(int cartId, int productId, int quantity);
        Task<bool> RemoveItem(int cartId, int productId);
        Task<bool> ClearCart(int cartId);
    }
}