using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Report
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public int ReportTypeId { get; set; }

    public int UserAccountId { get; set; }

    public virtual ReportType ReportType { get; set; } = null!;

    public virtual UserAccount UserAccount { get; set; } = null!;
}
