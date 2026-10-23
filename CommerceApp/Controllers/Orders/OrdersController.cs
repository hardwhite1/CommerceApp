using Microsoft.AspNetCore.Mvc;
using CommerceApp.Contracts.Orders;
using CommerceApp.DTOs.Orders;

namespace CommerceApp.Controllers.Orders
{
    [Route("api/[controller]")]
    [ApiController]

    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpGet]

        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _ordersService.GetAllOrdersAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _ordersService.GetOrdersByIdAsync(id);

            if(order == null )
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrders(CreateOrderDto dto )
        {
            var order = await _ordersService.CreateOrderAsync(dto);

            return CreatedAtAction(
                nameof(GetOrderById),
                new{id = order.Id}, order
            );
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> CancellOrder(int id)
        {
            var order = await _ordersService.CancelOrderAsync(id);

            return Ok(order);
        }

    }
}