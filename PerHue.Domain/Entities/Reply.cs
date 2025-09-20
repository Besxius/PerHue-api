using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Reply
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public int Reaction { get; set; }

    public int PostId { get; set; }

    public int ReplyId { get; set; }

    public virtual ICollection<Reply> InverseReplyNavigation { get; set; } = new List<Reply>();

    public virtual Post Post { get; set; } = null!;

    public virtual Reply ReplyNavigation { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
