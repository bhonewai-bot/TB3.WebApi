namespace TB3.WebApi.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpGet("{pageNo}/{pageSize}")]
        public async Task<IActionResult> GetProductCategories(int pageNo = 1, int pageSize = 10)
        {
            var result = await _productCategoryService.GetProductCategories(pageNo, pageSize);
            
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductCategory(ProductCategoryCreateRequestDto request)
        {
            var result = await _productCategoryService.CreateProductCategory(request);

            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);

            return Ok(result.Data);
        }
    }
}
