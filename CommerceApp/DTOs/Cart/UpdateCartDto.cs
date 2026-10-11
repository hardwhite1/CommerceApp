using CommerceApp.Models;

namespace CommerceApp.DTOs.Cart
{
    public class UpdateCartDto
    {
        public int Id { get; set; }
        public ICollection<CartItem> CartItems{ get; set; } = new List<CartItem>();
    }
}