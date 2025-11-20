using System;
using System.Collections.Generic;

namespace TB3.Database.AppDbContextModels;

public partial class TblCustomer
{
    public int CustomerId { get; set; }

    public string CustomerCode { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string? MobileNo { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }
}
