using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class TestResponse
{
    public int Id { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? Rating { get; set; }

    public string BestColor { get; set; } = null!;

    public string WorstColor { get; set; } = null!;

    public int ColorTypeId { get; set; }

    public int ExpertId { get; set; }

    public int TestRequestId { get; set; }

    public virtual ColorType ColorType { get; set; } = null!;

    public virtual Expert Expert { get; set; } = null!;

    public virtual TestRequest TestRequest { get; set; } = null!;
}
