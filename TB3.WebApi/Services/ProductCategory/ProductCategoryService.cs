using TB3.Models;

namespace TB3.WebApi.Services.ProductCategory;

public class ProductCategoryService : IProductCategoryService
{
    private readonly AppDbContext _db;

    public ProductCategoryService(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<List<ProductCategoryResponseDto>> GetProductCategories()
    {
        var categories = await _db.TblProductCategories
            .AsNoTracking()
            .OrderByDescending(x => x.ProductCategoryId)
            .Select(x => new ProductCategoryResponseDto
            {
                ProductCategoryId = x.ProductCategoryId,
                ProductCategoryCode = x.ProductCategoryCode,
                ProductCategoryName = x.ProductCategoryName
            })
            .ToListAsync();
        
        return categories;
    }

    public async Task<Result<ProductCategoryResponseDto>> CreateProductCategory(ProductCategoryCreateRequestDto request)
    {
        try
        {
            var exists = await _db.TblProductCategories
                .AnyAsync(x => x.ProductCategoryCode == request.ProductCategoryCode);

            if (exists)
                return Result<ProductCategoryResponseDto>.ValidationError("Product category code already exists");
            
            var category = new TblProductCategory
            {
                ProductCategoryCode = request.ProductCategoryCode,
                ProductCategoryName = request.ProductCategoryName
            };

            _db.TblProductCategories.Add(category);
            await _db.SaveChangesAsync();

            var categoryDto = new ProductCategoryResponseDto
            {
                ProductCategoryId = category.ProductCategoryId,
                ProductCategoryCode = category.ProductCategoryCode,
                ProductCategoryName = category.ProductCategoryName
            };
            
            return Result<ProductCategoryResponseDto>.Success(categoryDto, "Product category created.");
        }
        catch (Exception ex)
        {
            return Result<ProductCategoryResponseDto>.SystemError(ex.Message);
        }
    }
}