using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommerceApp.Contracts.Products;

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
        public IActionResult GetAllProducts()
        {
            return Ok();
        }
    }
}
