using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommerceApp.Contracts.Products;
using CommerceApp.DTOs.Products;

namespace CommerceApp.Controllers.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService) 
        { 
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if(product == null)
            {
                return NotFound("Requested product does not exist");
            }
            return Ok(product);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(
                nameof(GetProductById),
                new{ id = product.Id }, product
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            if(product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.DeleteProductAsync(id);
            
            return Ok(product);
        }
    }
}
