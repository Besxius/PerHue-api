using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpireDate { get; set; }

    public int UserAccountId { get; set; }

    public virtual UserAccount UserAccount { get; set; } = null!;
}
