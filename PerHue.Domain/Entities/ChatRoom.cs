using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class ChatRoom
{
    public int Id { get; set; }

    public int Person1 { get; set; }

    public int Person2 { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual UserAccount Person1Navigation { get; set; } = null!;

    public virtual UserAccount Person2Navigation { get; set; } = null!;
}
