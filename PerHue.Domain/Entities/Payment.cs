using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Lưu thông tin giao dịch thanh toán của người dùng, bao gồm số tiền, mô tả, trạng thái, mã giao dịch và liên kết với đăng ký dịch vụ.
/// </summary>
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

    public virtual UserSubscription IdNavigation { get; set; } = null!;

    public virtual ICollection<PaymentLog> PaymentLogs { get; set; } = new List<PaymentLog>();

    public virtual UserAccount User { get; set; } = null!;
}
