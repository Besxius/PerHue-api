using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Picture
{
    public int Id { get; set; }

    public string Source { get; set; } = null!;

    public int TestRequestId { get; set; }

    public virtual TestRequest TestRequest { get; set; } = null!;
}
