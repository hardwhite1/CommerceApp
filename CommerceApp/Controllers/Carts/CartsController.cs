using CommerceApp.DTOs.Cart;
using CommerceApp.Contracts.Carts;
using Microsoft.AspNetCore.Mvc;

namespace CommerceApp.Controllers.Carts
{
    [Route("api/[controller]")]
    [ApiController]

    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            var carts = await _cartService.GetAllCarts();

            return Ok(carts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            var cart = await _cartService.GetCartById(id);

            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> CreayeCart(int cartId, int productId, int quantity)
        {
            var cart = await _cartService.AddItem(cartId, productId, quantity);
            return CreatedAtAction(
                nameof(GetCartById),
                new{cartId = cart.Id}, cart
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int cartId, int productId, int quantity)
        {
            var cart = await _cartService.UpdateItemQuantity(cartId, productId, quantity);
            if(cart == null)
            {
                return NotFound();
            }
            return Ok(cart);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveItem(int cartId, int productId)
        {
            var item = await _cartService.RemoveItem(cartId, productId);

            return Ok(item);
        }

        [HttpDelete("{cartId}/items")]
        public async Task<IActionResult> ClearCart(int cartId)
        {
            var cart = await _cartService.ClearCart(cartId);

            return Ok(cart);
        }
    }
}
