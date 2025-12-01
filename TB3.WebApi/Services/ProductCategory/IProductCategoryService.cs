

namespace TB3.WebApi.Services.ProductCategory;

public interface IProductCategoryService
{
    Task<List<ProductCategoryResponseDto>> GetProductCategories();
    Task<ProductCategoryResponseDto?> CreateProductCategory(ProductCategoryCreateRequestDto request);
}