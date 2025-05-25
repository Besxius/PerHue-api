using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int Amount { get; set; }

    public string? Type { get; set; }

    public string? Message { get; set; }

    public DateTime Time { get; set; }

    public string Status { get; set; } = null!;

    public int UserId { get; set; }

    public virtual UserAccount User { get; set; } = null!;
}
