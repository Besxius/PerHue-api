using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class TestResult
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Picture { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual UserAccount User { get; set; } = null!;

    public virtual ICollection<Color> Colors { get; set; } = new List<Color>();
}
