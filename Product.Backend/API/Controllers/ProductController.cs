using Microsoft.AspNetCore.Mvc;
using Product.Backend.Application.Contracts.Services;
using Product.Backend.Application.Dto;

namespace Product.Backend.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService) 
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody]ProductDto product)
        {
            var createdProduct = await _productService.CreateProductsAsync(product);
            if (createdProduct == null)
            {
                // This might indicate a server or validation issue deeper in the service
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create product.");
            }
            return CreatedAtAction(nameof(GetProductById),new {id = createdProduct.Id}, createdProduct);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById([FromRoute]int id)
        {
            var product = await _productService.GetProductById(id);
            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetProducts([FromQuery] int pageNumber, int pageSize)
        {
            var pagedProducts = await _productService.GetPagedProductsAsync(pageNumber, pageSize);
            return Ok(pagedProducts);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute]int id, [FromBody] ProductDto product)
        {
            if (id != product.Id)
            {
                return BadRequest("ID mismatch between route and body.");
            }

            var success = await _productService.UpdateProductAsync(product);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute]int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (deleted is false )
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("add-to-stock/{id}/{quantity}")]
        public async Task<IActionResult> IncreaseProductStock([FromRoute]int id,[FromRoute] int quantity)
        {
            var success = await _productService.IncreaseStockAsync(id, quantity);
            if (success is false)
            {
                return NotFound();
            }

            return Ok(new { Message = $"Stock increased by {quantity} for Product ID {id}." });
        }

        [HttpPut("decrement-to-stock/{id}/{quantity}")]
        public async Task<IActionResult> DecreaseProductStock([FromRoute] int id, [FromRoute] int quantity)
        {
            var success = await _productService.DecreaseStockAsync(id, quantity);
            if (success is false)
            {
                return NotFound();
            }

            return Ok(new { Message = $"Stock decreased by {quantity} for Product ID {id}." });
        }
    }
}
