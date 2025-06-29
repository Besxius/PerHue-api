using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class CapsulePalette
{
    public int Id { get; set; }

    public int ColorTypeId { get; set; }

    public virtual ColorType ColorType { get; set; } = null!;

    public virtual ICollection<Color> Colors { get; set; } = new List<Color>();

    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
