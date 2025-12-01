namespace TB3.WebApi.Services.Product;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetProducts(string? productCategoryCode);
    Task<ProductResponseDto?> GetProduct(int id);
    Task<ProductResponseDto?> CreateProduct(ProductCreateRequestDto request);
    Task<ProductResponseDto?> PatchProduct(int id, ProductPatchRequestDto request);
    Task<bool> DeleteProduct(int id);
}