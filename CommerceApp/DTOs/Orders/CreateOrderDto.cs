using System.ComponentModel.DataAnnotations;

namespace CommerceApp.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CartId { get; set; }
    }
}