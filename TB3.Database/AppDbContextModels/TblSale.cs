using System;
using System.Collections.Generic;

namespace TB3.Database.AppDbContextModels;

public partial class TblSale
{
    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public string CashierName { get; set; } = null!;

    public string PaymentType { get; set; } = null!;

    public decimal? TotalAmount { get; set; }

    public virtual TblProduct Product { get; set; } = null!;
}
