using CommerceApp.Contracts.Carts;
using Microsoft.AspNetCore.Mvc;
using CommercialApp.DTOs.Cart;

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

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartById(int cartId)
        {
            var cart = await _cartService.GetCartById(cartId);

            return cart == null ? NotFound() :  Ok(cart);
        }

        [HttpPost("{cartId}/items")]
        public async Task<IActionResult> AddItem(int cartId, AddCartItemDto dto)
        {
            var cart = await _cartService.AddItem(cartId, dto.ProductId, dto.Quantity);
            return CreatedAtAction(
                nameof(GetCartById),
                new{cartId = cart.Id}, cart
            );
        }

        [HttpPut("{cartId}/items/{productId}")]
        public async Task<IActionResult> UpdateCart(int cartId, int productId, [FromBody] UpdateCartItemQuantityDto dto)
        {
            var cart = await _cartService.UpdateItemQuantity(cartId, productId, dto.Quantity);
            
            return cart == null ? NotFound() : Ok(cart);

        }

        [HttpDelete("{cartId}/items/{productId}")]
        public async Task<IActionResult> RemoveItem(
            int cartId, int productId)
        {
            var removed = await _cartService.RemoveItem(cartId, productId);

            return removed ? NoContent() : NotFound();
        }

        [HttpDelete("{cartId}/items")]
        public async Task<IActionResult> ClearCart(int cartId)
        {
            var cleared = await _cartService.ClearCart(cartId);

            return cleared ? NoContent() : NotFound();
        }
    }
}
