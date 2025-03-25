using System;
using System.Collections.Generic;

namespace Ebay.Backend.Entities;

public partial class ProductListCategory
{
    public int Id { get; set; }

    public string ProductName { get; set; } = null!;

    public string Category { get; set; } = null!;

    public decimal Price { get; set; }
}
