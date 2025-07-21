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
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody]ProductDto product)
        {
            var createdProduct = await _productService.CreateProductsAsync(product);
            if (createdProduct is null)
            {
                _logger.LogError("Product creation failed for: {@Product}", product);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create product.");
            }

            _logger.LogInformation("Product created successfully with ID {Id}", createdProduct.Id);
            return CreatedAtAction(nameof(GetProductById),new {id = createdProduct.Id}, createdProduct);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            _logger.LogInformation("Retrieved {Count} products", products?.Count() ?? 0);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById([FromRoute]int id)
        {
            var product = await _productService.GetProductById(id);
            if (product is null)
            {
                _logger.LogWarning("Product with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Product with ID {Id} found", id);
            return Ok(product);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] int pageNumber, int pageSize)
        {
            var pagedProducts = await _productService.GetPagedProductsAsync(pageNumber, pageSize);
            _logger.LogInformation("Retrieved {Count} products for page {PageNumber}", pagedProducts?.Count() ?? 0, pageNumber);
            return Ok(pagedProducts);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductDto product)
        {
            if (id != product.Id)
            {
                _logger.LogWarning("Product ID mismatch: route ID {RouteId} vs body ID {BodyId}", id, product.Id);
                return BadRequest("ID mismatch between route and body.");
            }

            var success = await _productService.UpdateProductAsync(product);
            if (!success)
            {
                _logger.LogWarning("Product with ID {Id} not found for update", id);
                return NotFound();
            }

            _logger.LogInformation("Product with ID {Id} updated successfully", id);
            return Ok(new { Message = "Product was updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute]int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (deleted is false )
            {
                _logger.LogWarning("Product with ID {Id} not found for deletion", id);
                return NotFound();
            }

            _logger.LogInformation("Product with ID {Id} deleted successfully", id);
            return Ok(new { Message = "Product was deleted successfully" });
        }

        [HttpPut("add-to-stock/{id}/{quantity}")]
        public async Task<IActionResult> IncreaseProductStock([FromRoute]int id,[FromRoute] int quantity)
        {
            var success = await _productService.IncreaseStockAsync(id, quantity);
            if (success is false)
            {
                _logger.LogWarning("Failed to increase stock. Product ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully increased stock for Product ID {Id} by {Quantity}", id, quantity);
            return Ok(new { Message = $"Stock increased by {quantity} for Product ID {id}." });
        }

        [HttpPut("decrement-to-stock/{id}/{quantity}")]
        public async Task<IActionResult> DecreaseProductStock([FromRoute] int id, [FromRoute] int quantity)
        {
            var success = await _productService.DecreaseStockAsync(id, quantity);
            if (success is false)
            {
                _logger.LogWarning("Failed to decrease stock. Product ID {Id} not found or insufficient stock", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully decreased stock for Product ID {Id} by {Quantity}", id, quantity);
            return Ok(new { Message = $"Stock decreased by {quantity} for Product ID {id}." });
        }
    }
}
