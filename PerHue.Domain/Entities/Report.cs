using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Report
{
    public int Id { get; set; }

    public string? Content { get; set; }

    public string? Type { get; set; }

    public string Status { get; set; } = null!;

    public string? Notice { get; set; }

    public int UserAccountId { get; set; }

    public virtual UserAccount UserAccount { get; set; } = null!;
}
