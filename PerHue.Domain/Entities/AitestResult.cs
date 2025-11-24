using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Lưu kết quả phân tích màu sắc tự động bởi AI, bao gồm màu nên dùng, màu nên tránh, loại màu phù hợp và ghi chú.
/// </summary>
public partial class AiTestResult
{
    public int Id { get; set; }

    public string? Note { get; set; }

    public DateTime? Date { get; set; }

    public string SuggestedColor { get; set; } = null!;

    public string AvoidedColor { get; set; } = null!;

    public int ColorTypeId { get; set; }

    public virtual ColorType ColorType { get; set; } = null!;

    public virtual TestRequest IdNavigation { get; set; } = null!;
}
