using Amazon.DynamoDBv2.DataModel;
using CRUD.DynamoDB.Tables;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.DynamoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IDynamoDBContext _context;
        public ProductController(IDynamoDBContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/{barcode}")]
        public async Task<IActionResult> Get(string id, string barcode)
        {
            var product = await _context.LoadAsync<Product>(id, barcode);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.ScanAsync<Product>(default).GetRemainingAsync();
            if (products == null || !products.Any())
            {
                return NotFound("No data");
            }
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product request)
        {
            var existingProduct = await _context.LoadAsync<Product>(request.Id, request.Barcode);
            if (existingProduct != null)
            {
                return Conflict("Product already exists");
            }
            await _context.SaveAsync(request);
            return Ok(request);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Product request)
        {
            var product = await _context.LoadAsync<Product>(request.Id, request.Barcode);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            await _context.SaveAsync(request);
            return Ok(request);
        }

        [HttpDelete("{id}/{barcode}")]
        public async Task<IActionResult> Delete(string id, string barcode)
        {
            var product = await _context.LoadAsync<Product>(id, barcode);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            await _context.DeleteAsync(product);
            return NoContent();
        }
    }
}
