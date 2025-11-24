using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Ghi lại lịch sử thay đổi trạng thái thanh toán, theo dõi các sự kiện và metadata liên quan.
/// </summary>
public partial class PaymentLog
{
    public int Id { get; set; }

    public int PaymentId { get; set; }

    public string EventType { get; set; } = null!;

    public string? OldStatus { get; set; }

    public string? NewStatus { get; set; }

    public string Mesage { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
