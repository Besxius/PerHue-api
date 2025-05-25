using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Color
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string HexCode { get; set; } = null!;

    public virtual ICollection<Avoid> Avoids { get; set; } = new List<Avoid>();

    public virtual ICollection<CapsulePalette> CapsulePalettes { get; set; } = new List<CapsulePalette>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Suggested> Suggesteds { get; set; } = new List<Suggested>();
}
