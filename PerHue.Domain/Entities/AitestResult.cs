using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class AiTestResult
{
    public int Id { get; set; }

    public string? Note { get; set; }

    public DateTime? Date { get; set; }

    public string SuggestedColor { get; set; } = null!;

    public string AvoidedColor { get; set; } = null!;

    public int ColorTypeId { get; set; }

    public virtual ColorType ColorType { get; set; } = null!;

    public virtual TestRequest IdNavigation { get; set; } = null!;
}
