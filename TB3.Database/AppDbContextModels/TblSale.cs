using System;
using System.Collections.Generic;

namespace TB3.Database.AppDbContextModels;

public partial class TblSale
{
    public int SaleId { get; set; }

    public string VoucherNo { get; set; } = null!;

    public string? PaymentType { get; set; }

    public decimal TotalAmount { get; set; }

    public string? StaffCode { get; set; }

    public DateTime SaleDateTime { get; set; }
}
