using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class SuggestedColor
{
    public int Id { get; set; }

    public string Hexcode { get; set; } = null!;

    public int AitestResultId { get; set; }

    public virtual AitestResult AitestResult { get; set; } = null!;
}
