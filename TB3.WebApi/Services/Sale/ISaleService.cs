namespace TB3.WebApi.Services.Sale;

public interface ISaleService
{
    Task<List<SaleResponseDto>> GetSales(DateTime? from, DateTime? to);
    Task<SaleResponseDto?> GetSale(string voucherNo);
    Task<SaleResponseDto?> CreateSale(SaleCreateRequestDto request);
}