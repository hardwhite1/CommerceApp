using CommerceApp.Data;
using CommerceApp.DTOs.Orders;
using CommerceApp.Models;
using CommerceApp.Contracts.Orders;
using Microsoft.EntityFrameworkCore;

namespace CommerceApp.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            //retrieve the cart and its items
            var cart = await _context.Carts.Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                        .FirstOrDefaultAsync(c => c.Id == dto.CartId);

            if (cart == null)
            {
                throw new ArgumentException($"Cart {dto.CartId} was not found");
            }

            if (!cart.CartItems.Any())
            {
                throw new InvalidOperationException("Cannot create an order from an empty cart");
            }

            //validate stock and calculate the total
            decimal totalAmount = 0;
            foreach (var item in cart.CartItems)
            {
                if (item.Quantity <= 0)
                {
                    throw new InvalidOperationException("Cart item quantity must be greater than zero");
                }

                if (item.Product.StockQuantity < item.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for {item.Product.Name}.");

                }

                totalAmount += item.Quantity * item.Product.Price;
            }
            //Build the order and snapshot product prices
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                Status = "pending",
                TotalAmount = totalAmount
            };

            foreach (var item in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });
            }
            _context.Orders.Add(order);
            //Map the saved order to its response DTO
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,

                OrderItems = order.OrderItems.Select(item =>
                new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };
        }
    }




}