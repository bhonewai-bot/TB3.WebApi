namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? productCategoryCode)
        {
           var products = await _productService.GetProducts(productCategoryCode);
           return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProduct(id);

            if (product is null)
            {
                return NotFound("Product not found");
            }
            
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateRequestDto request)
        {
            var response = await _productService.CreateProduct(request);
            return Ok(response);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(int id, ProductPatchRequestDto request)
        {
            var response = await _productService.PatchProduct(id, request);
            if (response is null)
            {
                return NotFound("Product not found");
            }
            
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.DeleteProduct(id);
            return product ? NoContent() : NotFound("Product not found");
        }
    }
}