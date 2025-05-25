using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Palette
{
    public int Id { get; set; }

    public virtual AiTestResult IdNavigation { get; set; } = null!;

    public virtual ICollection<CapsulePalette> CapsulePalettes { get; set; } = new List<CapsulePalette>();
}
