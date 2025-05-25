using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Carousel
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Picture { get; set; }

    public string? Color { get; set; }

    public int BrandId { get; set; }

    public virtual Brand Brand { get; set; } = null!;
}
