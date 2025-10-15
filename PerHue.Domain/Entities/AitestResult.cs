using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class AitestResult
{
    public int Id { get; set; }

    public string? Note { get; set; }

    public DateTime? Date { get; set; }

    public int ColorTypeId { get; set; }

    public virtual ICollection<AvoidedColor> AvoidedColors { get; set; } = new List<AvoidedColor>();

    public virtual ColorType ColorType { get; set; } = null!;

    public virtual TestRequest IdNavigation { get; set; } = null!;

    public virtual ICollection<SuggestedColor> SuggestedColors { get; set; } = new List<SuggestedColor>();
}
