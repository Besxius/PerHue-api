using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Lưu ảnh phân tích bởi AI kèm ghi chú cho yêu cầu test.
/// </summary>
public partial class AiPicture
{
    public int Id { get; set; }

    public string Source { get; set; } = null!;

    public string Note { get; set; } = null!;

    public int TestRequestId { get; set; }

    public virtual TestRequest TestRequest { get; set; } = null!;
}
