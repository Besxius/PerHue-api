using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Post
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public int Reaction { get; set; }

    public int View { get; set; }

    public DateTime Time { get; set; }

    public int UserId { get; set; }

    public int TopicId { get; set; }

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();

    public virtual Topic Topic { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
