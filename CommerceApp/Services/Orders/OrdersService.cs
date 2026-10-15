using CommerceApp.Data;
using CommerceApp.DTOs.Orders;
using CommerceApp.Models;
using CommerceApp.Contracts.Orders;
using Microsoft.EntityFrameworkCore;

namespace CommerceApp.Services.Orders
{
    public class OrderService : IOrdersService
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

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders.Include(o => o.OrderItems)
                         .ThenInclude(oi => oi.Product)
                         .AsNoTracking()
                         .ToListAsync();
            return orders.Select(MapToDto).ToList();
            
        }

        public async Task<OrderDto?> GetOrdersByIdAsync(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(o => o.Id == orderId);
            return order == null ? null : MapToDto(order);
            
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return false;
            }
            if (order.Status != "pending")
            {
                return false;
            }
            order.Status = "cancelled";
            
            await _context.SaveChangesAsync();

            return true;
        }

        private static OrderDto MapToDto(Order order)
        {
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
                    }
                ).ToList()

            };
        }
    }




}