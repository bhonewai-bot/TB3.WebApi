namespace TB3.WebApi.Services.Sale;

public interface ISaleService
{
    Task<Result<List<SaleResponseDto>>> GetSales(int pageNo, int pageSize);
    Task<Result<List<SaleResponseDto>>> GetSales(DateTime? from, DateTime? to);
    Task<Result<SaleResponseDto>> GetSale(string voucherNo);
    Task<Result<SaleResponseDto>> CreateSale(SaleCreateRequestDto request);
}