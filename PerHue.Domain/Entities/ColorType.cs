using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class ColorType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<CapsulePalette> CapsulePalettes { get; set; } = new List<CapsulePalette>();

    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
