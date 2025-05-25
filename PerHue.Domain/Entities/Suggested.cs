using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Suggested
{
    public int Id { get; set; }

    public virtual AiTestResult IdNavigation { get; set; } = null!;

    public virtual ICollection<Color> Colors { get; set; } = new List<Color>();
}
