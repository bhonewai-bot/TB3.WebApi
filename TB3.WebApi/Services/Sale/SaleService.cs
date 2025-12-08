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

    public async Task<Result<List<SaleResponseDto>>> GetSales(int pageNo, int pageSize)
    {
        try
        {
            if (pageNo <= 0)
                return Result<List<SaleResponseDto>>.ValidationError("Page No must be greater than zero");
            
            if (pageSize <= 0)
                return Result<List<SaleResponseDto>>.ValidationError("Page Size must be greater than zero");
            
            var sales = await _db.TblSales
                .AsNoTracking()
                .OrderByDescending(x => x.SaleId)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SaleResponseDto()
                {
                    SaleId = s.SaleId,
                    VoucherNo = s.VoucherNo,
                    PaymentType = s.PaymentType,
                    TotalAmount = s.TotalAmount,
                    StaffCode = s.StaffCode,
                    SaleDateTime = s.SaleDateTime,
                    SaleDetails =  _db.TblSaleDetails
                        .Where(d => d.VoucherNo == s.VoucherNo)
                        .Select(d => new SaleDetailResponseDto()
                        {
                            SaleDetailId = d.SaleDetailId,
                            VoucherNo = d.VoucherNo,
                            ProductCode = d.ProductCode,
                            Price = d.Price,
                            Quantity = d.Quantity,
                            Amount = d.Price * d.Quantity,
                        })
                        .ToList()
                })
                .ToListAsync();
            
            return Result<List<SaleResponseDto>>.Success(sales);
        }
        catch (Exception ex)
        {
            return Result<List<SaleResponseDto>>.SystemError(ex.Message);
        }
    }

    public async Task<Result<List<SaleResponseDto>>> GetSales(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            if (!startDate.HasValue && !endDate.HasValue)
                return Result<List<SaleResponseDto>>.ValidationError("Start date and End date is required");
       
            var query = _db.TblSales.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(x => x.SaleDateTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.SaleDateTime <= endDate.Value.AddDays(1));

            var sales = await query
                .AsNoTracking()
                .OrderBy(x => x.SaleId)
                .Select(s => new SaleResponseDto()
                {
                    SaleId = s.SaleId,
                    VoucherNo = s.VoucherNo,
                    TotalAmount = s.TotalAmount,
                    PaymentType = s.PaymentType,
                    StaffCode = s.StaffCode,
                    SaleDateTime = s.SaleDateTime,
                    SaleDetails = _db.TblSaleDetails
                        .Where(d => d.VoucherNo == s.VoucherNo)
                        .Select(d => new SaleDetailResponseDto()
                        {
                            SaleDetailId = d.SaleDetailId,
                            VoucherNo = d.VoucherNo,
                            ProductCode = d.ProductCode,
                            Price = d.Price,
                            Quantity = d.Quantity
                        })
                        .ToList()
                })
                .ToListAsync();
       
            return Result<List<SaleResponseDto>>.Success(sales);
        }
        catch (Exception ex)
        {
            return Result<List<SaleResponseDto>>.SystemError(ex.Message);
        }
    }

    public async Task<Result<SaleResponseDto>> GetSale(string voucherNo)
    {
        try
        {
            var sale = await _db.TblSales
                .AsNoTracking()
                .Where(x => x.VoucherNo == voucherNo)
                .Select(s => new SaleResponseDto()
                {
                    SaleId = s.SaleId,
                    VoucherNo = s.VoucherNo,
                    TotalAmount = s.TotalAmount,
                    PaymentType = s.PaymentType,
                    StaffCode = s.StaffCode,
                    SaleDateTime = s.SaleDateTime,
                    SaleDetails = _db.TblSaleDetails
                        .Where(d => d.VoucherNo == voucherNo)
                        .Select(d => new SaleDetailResponseDto()
                        {
                            SaleDetailId = d.SaleDetailId,
                            VoucherNo = d.VoucherNo,
                            ProductCode = d.ProductCode,
                            Price = d.Price,
                            Quantity = d.Quantity,
                            Amount = d.Price * d.Quantity,
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        
            if (sale is null)
                return Result<SaleResponseDto>.ValidationError("Sale not found");
            
            return Result<SaleResponseDto>.Success(sale);
        }
        catch (Exception ex)
        {
            return Result<SaleResponseDto>.SystemError(ex.Message);
        }
    }

    public async Task<Result<SaleResponseDto>> CreateSale(SaleCreateRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.PaymentType))
                return Result<SaleResponseDto>.ValidationError("Payment type is required");
            
            if (string.IsNullOrWhiteSpace(request.StaffCode))
                return Result<SaleResponseDto>.ValidationError("Staff code is required");
            
            var staffExists = await _db.TblStaffs
                .AnyAsync(x => x.StaffCode == request.StaffCode);
            
            if (!staffExists)
                return Result<SaleResponseDto>.ValidationError("Invalid Staff Code");
            
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
            foreach (var detailRequest in request.SaleDetails)
            {
                var product = await _db.TblProducts
                    .FirstOrDefaultAsync(x => x.ProductCode == detailRequest.ProductCode);
                
                if (product is null)
                    return Result<SaleResponseDto>.ValidationError("Product not found");
                
                if (product.Quantity < detailRequest.Quantity)
                    return Result<SaleResponseDto>.ValidationError("Insufficient stock for this product");

                product.Quantity -= detailRequest.Quantity;
                product.ModifiedDateTime = DateTime.Now;

                saleDetail.Add(new TblSaleDetail()
                {
                    VoucherNo = voucherNo,
                    ProductCode = detailRequest.ProductCode,
                    Price = detailRequest.Price,
                    Quantity = detailRequest.Quantity,
                });
            }
            
            await _db.TblSaleDetails.AddRangeAsync(saleDetail);
            await _db.SaveChangesAsync();

            var saleDto = new SaleResponseDto()
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
            
            return Result<SaleResponseDto>.Success(saleDto);
        }
        catch (Exception ex)
        {
            return Result<SaleResponseDto>.SystemError(ex.Message);
        }
    }
}