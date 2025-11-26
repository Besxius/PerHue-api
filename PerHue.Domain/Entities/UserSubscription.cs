using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class UserSubscription
{
    public int Id { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool Status { get; set; }

    public short RemainingUses { get; set; }

    public int UserId { get; set; }

    public int ServicePackageId { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual ServicePackage ServicePackage { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
