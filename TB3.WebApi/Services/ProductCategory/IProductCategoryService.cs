using TB3.Models;

namespace TB3.WebApi.Services.ProductCategory;

public interface IProductCategoryService
{
    Task<Result<List<ProductCategoryResponseDto>>> GetProductCategories(int pageNo, int pageSize);
    Task<Result<ProductCategoryResponseDto>> CreateProductCategory(ProductCategoryCreateRequestDto request);
}