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

        [HttpGet]
        public async Task<IActionResult> GetProductCategories()
        {
            var categories = await _productCategoryService.GetProductCategories();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductCategory(ProductCategoryCreateRequestDto request)
        {
            var result = await _productCategoryService.CreateProductCategory(request);

            if (result is null)
            {
                return BadRequest("Category code already exists");
            }

            return Ok(result);
        }
    }
}
