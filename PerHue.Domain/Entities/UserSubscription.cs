using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class UserSubscription
{
    public int Id { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public short? Duration { get; set; }

    public int UserId { get; set; }

    public int ServicePackageId { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ServicePackage ServicePackage { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
