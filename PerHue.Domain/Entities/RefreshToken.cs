using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Lưu token làm mới phiên đăng nhập của người dùng với thời gian hết hạn.
/// </summary>
public partial class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpireDate { get; set; }

    public int UserAccountId { get; set; }

    public virtual UserAccount UserAccount { get; set; } = null!;
}
