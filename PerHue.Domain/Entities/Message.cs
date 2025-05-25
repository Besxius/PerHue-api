using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Message
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime Time { get; set; }

    public string Status { get; set; } = null!;

    public int Sender { get; set; }

    public int ChatRoomId { get; set; }

    public virtual ChatRoom ChatRoom { get; set; } = null!;

    public virtual UserAccount SenderNavigation { get; set; } = null!;
}
