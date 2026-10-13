namespace CommerceApp.DTOs.Orders
{
    public class OrderItemDto
    {
        public int ProductId {get; set;}
        public string ProductName {get; set;} = string.Empty;
        public int Quantity {get; set;}
        public decimal UnitPrice {get; set;}
        public decimal SubTotal => Quantity * UnitPrice;
    }
}

//This DTo is used to return each purchased product's details in the order summary. It includes the product ID, name, quantity purchased, unit price, and calculates the subtotal for that item.