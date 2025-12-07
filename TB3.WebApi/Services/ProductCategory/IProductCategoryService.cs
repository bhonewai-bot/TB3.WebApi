using TB3.Models;

namespace TB3.WebApi.Services.ProductCategory;

public interface IProductCategoryService
{
    Task<List<ProductCategoryResponseDto>> GetProductCategories();
    Task<Result<ProductCategoryResponseDto>> CreateProductCategory(ProductCategoryCreateRequestDto request);
}