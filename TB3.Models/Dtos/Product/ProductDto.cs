namespace TB3.Models.Dtos.Product;

public class ProductDto
{
    public int ProductId { get; set; }
    
    public string ProductCode { get; set; }

    public string ProductName { get; set; }
    
    public decimal Price { get; set; }

    public int Quantity { get; set; }
    
    public string ProductCategoryCode { get; set; }
}

public class ProductGetResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public List<ProductDto> Products { get; set; }
}

public class ProductGetByCodeResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public ProductDto Product { get; set; }
}

public class ProductResponseDto : ProductGetByCodeResponseDto
{
    
}

public class ProductCreateRequestDto
{
    public string ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
    
    public string ProductCategoryCode { get; set; }
}

public class ProductUpdateRequestDto
{
    public string ProductCode { get; set; }
    
    public string ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
    
    public string ProductCategoryCode { get; set; }
}

public class ProductPatchRequestDto
{
    public string? ProductCode { get; set; }
    
    public string? ProductName { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }
    
    public string? ProductCategoryCode { get; set; }
}