using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class ProductPicture
{
    public int Id { get; set; }

    public string Picture { get; set; } = null!;

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
