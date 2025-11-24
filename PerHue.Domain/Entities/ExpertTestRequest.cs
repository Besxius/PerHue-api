using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Bảng trung gian quản lý việc gán chuyên gia cho yêu cầu test, theo dõi trạng thái xử lý.
/// </summary>
public partial class ExpertTestRequest
{
    public int ExpertId { get; set; }

    public int TestRequestId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual Expert Expert { get; set; } = null!;

    public virtual TestRequest TestRequest { get; set; } = null!;
}
