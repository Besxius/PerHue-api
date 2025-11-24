using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Lưu ảnh người dùng gửi lên kèm theo yêu cầu test.
/// </summary>
public partial class Picture
{
    public int Id { get; set; }

    public string Source { get; set; } = null!;

    public int TestRequestId { get; set; }

    public virtual TestRequest TestRequest { get; set; } = null!;
}
