using eshop.Catalog.Application;
using Microsoft.AspNetCore.Mvc;

namespace eshop.Catalog.API.Controllers
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
        public IActionResult GetProducts()
        {
            var result = _productService.GetProducts();
            return Ok(result);
        }
    }
}
