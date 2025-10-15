using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class AvoidedColor
{
    public int Id { get; set; }

    public string? Hexcode { get; set; }

    public int AitestResultId { get; set; }

    public virtual AitestResult AitestResult { get; set; } = null!;
}
