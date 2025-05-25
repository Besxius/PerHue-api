using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Notification
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime Time { get; set; }

    public int Receiver { get; set; }

    public virtual UserAccount ReceiverNavigation { get; set; } = null!;
}
