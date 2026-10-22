namespace CommerceApp.DTOs.Orders
{
    public class OrderDto
    {
        public int Id {get; set; }
        public DateTime OrderDate {get; set; }
        public string Status {get; set;} = string.Empty;
        public decimal TotalAmount {get; set; }
        public ICollection<OrderItemDto> OrderItems {get; set;} = new List<OrderItemDto>();
    }
}

//This represents the complete order returned to angular including its items and total.