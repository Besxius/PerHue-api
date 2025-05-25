using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Brand
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Slogan { get; set; }

    public string? Logo { get; set; }

    public string? Link { get; set; }

    public virtual ICollection<Carousel> Carousels { get; set; } = new List<Carousel>();

    public virtual UserAccount IdNavigation { get; set; } = null!;
}
