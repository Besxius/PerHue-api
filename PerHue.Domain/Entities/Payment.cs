using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int Amount { get; set; }

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Status { get; set; } = null!;

    public string TransactionId { get; set; } = null!;

    public int UserId { get; set; }

    public int UserSubscriptionId { get; set; }

    public virtual ICollection<PaymentLog> PaymentLogs { get; set; } = new List<PaymentLog>();

    public virtual UserAccount User { get; set; } = null!;

    public virtual UserSubscription UserSubscription { get; set; } = null!;
}
