using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Định nghĩa các vai trò/quyền hạn khác nhau trong hệ thống (ví dụ: Admin, User, Expert).
/// </summary>
public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
}
