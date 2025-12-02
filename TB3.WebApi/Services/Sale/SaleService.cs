namespace TB3.WebApi.Services.Sale;

public class SaleService : ISaleService
{
    private readonly AppDbContext _db;
    private readonly ISequenceService _sequenceService;

    public SaleService(AppDbContext db, ISequenceService sequenceService)
    {
        _db = db;
        _sequenceService = sequenceService;
    }

    public async Task<List<SaleResponseDto>> GetSales(DateTime? startDate, DateTime? endDate)
    {
       var query = _db.TblSales.AsQueryable();

       if (startDate.HasValue)
       {
           query = query.Where(x => x.SaleDateTime >= startDate.Value);
       }

       if (endDate.HasValue)
       {
           query = query.Where(x => x.SaleDateTime <= endDate.Value.AddDays(1));
       }

       List<SaleResponseDto> sales = await query
           .AsNoTracking()
           .OrderBy(x => x.SaleId)
           .Select(x => new SaleResponseDto()
           {
               SaleId = x.SaleId,
               VoucherNo = x.VoucherNo,
               TotalAmount = x.TotalAmount,
               PaymentType = x.PaymentType,
               StaffCode = x.StaffCode,
               SaleDateTime = x.SaleDateTime,
               SaleDetails = null
           })
           .ToListAsync();
       
       return sales;
    }

    public async Task<SaleResponseDto?> GetSale(string voucherNo)
    {
        var sale = await _db.TblSales
            .AsNoTracking()
            .Where(x => x.VoucherNo == voucherNo)
            .Select(x => new SaleResponseDto()
            {
                SaleId = x.SaleId,
                VoucherNo = x.VoucherNo,
                TotalAmount = x.TotalAmount,
                PaymentType = x.PaymentType,
                StaffCode = x.StaffCode,
                SaleDateTime = x.SaleDateTime,
                SaleDetails = _db.TblSaleDetails
                    .Where(x => x.VoucherNo == voucherNo)
                    .Select(y => new SaleDetailResponseDto()
                    {
                        SaleDetailId = y.SaleDetailId,
                        VoucherNo = y.VoucherNo,
                        ProductCode = y.ProductCode,
                        Price = y.Price,
                        Quantity = y.Quantity,
                        Amount = y.Price * y.Quantity,
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
        
        return sale;
    }

    public async Task<SaleResponseDto?> CreateSale(SaleCreateRequestDto request)
    {
        string voucherNo = await _sequenceService.GenerateCode("VoucherNo");
        
        decimal totalAmount = request.SaleDetails
            .Sum(x => x.Price * x.Quantity);

        var sale = new TblSale()
        {
            VoucherNo = voucherNo,
            PaymentType = request.PaymentType,
            StaffCode = request.StaffCode,
            TotalAmount = totalAmount,
            SaleDateTime = DateTime.Now,
        };

        _db.TblSales.Add(sale);
        await _db.SaveChangesAsync();
        
        var saleDetail = new List<TblSaleDetail>();
        foreach (var detail in request.SaleDetails)
        {
            var product = await _db.TblProducts
                .FirstOrDefaultAsync(x => x.ProductCode == detail.ProductCode);

            if (product is null || product.Quantity < detail.Quantity)
            {
                return null; 
            }

            product.Quantity -= detail.Quantity;
            product.ModifiedDateTime = DateTime.Now;

            saleDetail.Add(new TblSaleDetail()
            {
                VoucherNo = voucherNo,
                ProductCode = detail.ProductCode,
                Price = detail.Price,
                Quantity = product.Quantity,
            });
        }
        
        await _db.TblSaleDetails.AddRangeAsync(saleDetail);
        await _db.SaveChangesAsync();

        return new SaleResponseDto()
        {
            SaleId = sale.SaleId,
            VoucherNo = sale.VoucherNo,
            PaymentType = sale.PaymentType,
            StaffCode = sale.StaffCode,
            TotalAmount = sale.TotalAmount,
            SaleDateTime = sale.SaleDateTime,
            SaleDetails = saleDetail.Select(x => new SaleDetailResponseDto()
            {
                SaleDetailId = x.SaleDetailId,
                VoucherNo = x.VoucherNo,
                ProductCode = x.ProductCode,
                Price = x.Price,
                Quantity = x.Quantity,
                Amount = x.Amount ?? sale.TotalAmount,
            }).ToList()
        };
    }
}