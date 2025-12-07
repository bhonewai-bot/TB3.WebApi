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

        [HttpGet("{pageNo}/{pageSize}")]
        public async Task<IActionResult> GetProducts(int pageNo, int pageSize)
        {
           var result = await _productService.GetProducts(pageNo, pageSize);
           if (!result.IsSuccess)
               return BadRequest(result.Message);
           
           return Ok(result);
        }

        [HttpGet("{productCode}")]
        public async Task<IActionResult> GetProduct(string productCode)
        {
            var result = await _productService.GetProductByCode(productCode);

            if (!result.IsSuccess)
                return BadRequest(result.Message);
            
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateRequestDto request)
        {
            var result = await _productService.CreateProduct(request);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateRequestDto request)
        {
            var result = await _productService.UpdateProduct(id, request);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(int id, ProductPatchRequestDto request)
        {
            var result = await _productService.PatchProduct(id, request);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            
            return Ok(result);
        }
    }
}