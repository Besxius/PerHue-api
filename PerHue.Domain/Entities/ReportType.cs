using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class ReportType
{
    public int Id { get; set; }

    public string? Content { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
