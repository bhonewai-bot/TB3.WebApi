using TB3.Models;

namespace TB3.WebApi.Services.Product;

public interface IProductService
{
    Task<ProductGetResponseDto> GetProducts(int pageNo, int pageSize);
    Task<ProductGetByCodeResponseDto> GetProductByCode(string productCode);
    Task<ProductResponseDto> CreateProduct(ProductCreateRequestDto request);
    Task<ProductResponseDto> UpdateProduct(int id, ProductUpdateRequestDto request);
    Task<ProductResponseDto> PatchProduct(int id, ProductPatchRequestDto request);
    Task<ProductResponseDto> DeleteProduct(int id);
}