using System;
using System.Collections.Generic;

namespace TB3.Database.AppDbContextModels;

public partial class TblSale
{
    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public virtual TblProduct Product { get; set; } = null!;
}
