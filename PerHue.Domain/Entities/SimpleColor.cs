using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class SimpleColor
{
    public int Id { get; set; }

    public string Hexcode { get; set; } = null!;

    public int TestResultId { get; set; }

    public virtual TestResult TestResult { get; set; } = null!;
}
