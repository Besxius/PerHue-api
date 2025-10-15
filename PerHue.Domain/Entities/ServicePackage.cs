using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class ServicePackage
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long Price { get; set; }

    public short Duration { get; set; }

    public short Uses { get; set; }

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
