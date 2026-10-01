using CommerceApp.Contracts.Products;
using CommerceApp.DTOs.Products;
using Microsoft.EntityFrameworkCore;
using CommerceApp.Models;
using CommerceApp.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CommerceApp.Services.Products
{
    public class ProductService: IProductService
    {

        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context) 
        { 
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();

            return products.Select(MapToDto);
            
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product //create a new product
            {
                Name = dto.Name,
                Description = dto.Description,
                Brand = dto.Brand,
                Category = dto.Category,
                Price = dto.Price,
                OriginalPrice = dto.OriginalPrice,
                StockQuantity = dto.StockQuantity,
                PictureUrl = dto.PictureUrl,
                InStock = dto.StockQuantity > 0

            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p=>p.Id == id);

            if ( existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;
            existingProduct.Brand = dto.Brand;
            existingProduct.Category = dto.Category;
            existingProduct.Price = dto.Price;
            existingProduct.OriginalPrice = dto.OriginalPrice;
            existingProduct.StockQuantity = dto.StockQuantity;
            existingProduct.PictureUrl = dto.PictureUrl;
            existingProduct.InStock = dto.StockQuantity > 0;

            await _context.SaveChangesAsync();

            return MapToDto(existingProduct);

        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if ( product == null )
            {
                return false;
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return true;
        }
        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                StockQuantity = product.StockQuantity,
                PictureUrl = product.PictureUrl,
                InStock = product.InStock
            };
        }
    }
}
