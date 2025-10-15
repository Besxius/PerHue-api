using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class ColorType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<AitestResult> AitestResults { get; set; } = new List<AitestResult>();

    public virtual ICollection<CapsulePalette> CapsulePalettes { get; set; } = new List<CapsulePalette>();

    public virtual ICollection<TestResponse> TestResponses { get; set; } = new List<TestResponse>();
}
