using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class TestResult
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Picture { get; set; }

    public DateTime CreatedDate { get; set; }

    public string ChosenColor { get; set; } = null!;

    public string SuggestedColor { get; set; } = null!;

    public int ColorTypeId { get; set; }

    public virtual ColorType ColorType { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
