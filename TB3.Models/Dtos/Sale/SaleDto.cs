namespace TB3.Models.Dtos.Sale;

public class SaleResponseDto
{
    public int SaleId { get; set; }
    public string VoucherNo { get; set; }
    public string? PaymentType { get; set; }
    public decimal TotalAmount { get; set; }
    public string? StaffCode { get; set; }
    public DateTime SaleDateTime { get; set; }

    public List<SaleDetailResponseDto>? SaleDetails { get; set; }
}

public class SaleDetailResponseDto
{
    public int SaleDetailId { get; set; }
    public string VoucherNo { get; set; }
    public string ProductCode { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
}

public class SaleCreateRequestDto
{
    public string? PaymentType { get; set; }
    public string? StaffCode { get; set; }
    public List<SaleDetailCreateRequestDto> SaleDetails { get; set; } = new List<SaleDetailCreateRequestDto>();
}

public class SaleDetailCreateRequestDto
{
    public string ProductCode { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}