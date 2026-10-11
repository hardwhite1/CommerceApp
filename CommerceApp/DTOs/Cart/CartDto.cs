using CommerceApp.Models;

namespace CommerceApp.DTOs.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public ICollection<CartItemDto> CartItems{ get; set; } = new List<CartItemDto>();
    }
}