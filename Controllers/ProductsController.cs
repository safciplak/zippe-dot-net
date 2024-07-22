using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Repositories;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository repository, ILogger<ProductsController> logger)
        {
            _logger = logger;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAll()
        {
            return Ok(_repository.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetById(int id)
        {
            var product = _repository.GetById(id);
            if (product == null)
            {
                _logger.LogWarning("product with id {ProductId} not found", id);
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (product == null)
            {
                return BadRequest("product is null");
            }

            try
            {
                await _repository.AddAsync(product);
                return CreatedAtAction(nameof(Create), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"internal server error: {ex.Message}");
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,Product product)
        {
            if (id != product.Id)
            {
                _logger.LogWarning("id doesn't match: url id {UrlId} does not match body id {BodyId}", id, product.Id);
                return BadRequest("ID mismatch");
            }

            var existingProduct = _repository.GetById(id);
            if (existingProduct == null)
            {
                _logger.LogWarning("product with ID {ProductId} not found", id);
                return NotFound();
            }

            if (string.IsNullOrEmpty(product.Name) || product.Price <= 0)
            {
                _logger.LogWarning("invalid product data for id {ProductId}", id);
                return BadRequest("invalid product data");
            }

            await _repository.UpdateAsync(product);
            _logger.LogInformation("product id {ProductId} updated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _repository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            _repository.Delete(id);
            _logger.LogInformation("product with id {ProductId} deleted", id);
            return NoContent();
        }
    }
}
