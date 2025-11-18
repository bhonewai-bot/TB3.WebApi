using System;
using System.Collections.Generic;

namespace TB3.Database.AppDbContextModels;

public partial class TblProduct
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public int ProductCategoryId { get; set; }

    public string Sku { get; set; } = null!;

    public virtual ICollection<TblSale> TblSales { get; set; } = new List<TblSale>();
}
