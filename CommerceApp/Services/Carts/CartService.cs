using Microsoft.EntityFrameworkCore;
using CommerceApp.DTOs.Cart;
using CommerceApp.Models;
using CommerceApp.Data;
using CommerceApp.Contracts.Carts;

namespace CommerceApp.Services.Carts
{
    public class CartService: ICartService
    {
        private readonly ApplicationDbContext _dbContext;

        public CartService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CartDto>> GetAllCarts()
        {
            var carts = await _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .AsNoTracking()
                .ToListAsync();

            return carts.Select(MapToDto);
        }

        public async Task<CartDto?> GetCartById(int id)
        {
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return cart == null ? null : MapToDto(cart);
        }

        public async Task<CartDto> AddItem(int cartId, int productId, int quantity)
        {
            //1. Validate incoming quantity
            if(quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero");
            }

            //2. FInd the Cart
            var cart = await _dbContext.Carts
                        .Include(c => c.CartItems)
                        .FirstOrDefaultAsync(ci => ci.Id == cartId);
            if(cart == null)
            {
                throw new KeyNotFoundException($"Cart with ID {cartId} was not found");
            }

            //3. Find the product
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if(product == null)
            { 
                throw new KeyNotFoundException($"Product with ID {productId} does not exist");
            }

            //4. check whether the product is in stock
            var existingItem = cart.CartItems.FirstOrDefault(p => p.ProductId == productId);
            var currentQuantity = existingItem?.Quantity ?? 0;
            var newQuantity = currentQuantity + quantity;

            if(newQuantity > product.StockQuantity)
            {
                throw new InvalidOperationException($"Insufficient stock. Available quantity: {product.StockQuantity}. ");
            }

            //5. Add a new item or update the existing one
            if(existingItem == null)
            {
                var cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _dbContext.CartItems.Add(cartItem);
            }
            else
            {
                existingItem.Quantity = newQuantity;
            }

            //6. Persist the changes
            await _dbContext.SaveChangesAsync();

            //7. Reload the cart with its products for the response DTO
            var updatedCart = await _dbContext.Carts
                            .Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product)
                            .AsNoTracking()
                            .FirstAsync(c => c.Id == cartId);

            return MapToDto(updatedCart);

        }

        public async Task<CartDto?> UpdateItemQuantity(int cartId, int productId, int quantity)
        {
            //1. validate quantity
            if(quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero");
            }

            //2. Find the cart item and its product
            var cartItem = await _dbContext.CartItems.Include(ci => ci.Product)
                            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if(cartItem == null)
            {
                return null;
            }

            //3. Check available stock
            if(quantity > cartItem.Product.StockQuantity)
            {
                throw new InvalidOperationException($"Insufficient stock. Available quantity: " + $"{cartItem.Product.StockQuantity}");
            }

            //4. Update the quantity
            cartItem.Quantity = quantity;
            await _dbContext.SaveChangesAsync();

            //5. return the updated cart
            var updatedCart = await _dbContext.Carts.Include(c => c.CartItems)
                              .ThenInclude(ci => ci.Product)
                              .AsNoTracking()
                              .FirstOrDefaultAsync(c => c.Id == cartId);

            return updatedCart == null ? null : MapToDto(updatedCart);
        }

        public async Task<bool> RemoveItem(int cartId, int productId)
        {
            var cartItems = await _dbContext.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
            if(cartItems == null)
            {
                return false;
            }

            _dbContext.CartItems.Remove(cartItems);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearCart(int cartId)
        {
            var cart = await _dbContext.Carts.Include(c => c.CartItems)
                       .FirstOrDefaultAsync(c => c.Id == cartId);

            if(cart == null)
            {
                return false;
            }

            _dbContext.CartItems.RemoveRange(cart.CartItems);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        

        private static CartDto MapToDto(Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,
                CartItems = cart.CartItems.Select(item => new CartItemDto //for each cart item, create a new CartItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    Quantity = item.Quantity

                }).ToList()
            };
        }
        
    
        
    }
}