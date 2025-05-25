using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class AiTestResult
{
    public int Id { get; set; }

    public string Picture { get; set; } = null!;

    public string SkinColor { get; set; } = null!;

    public string HairColor { get; set; } = null!;

    public string EyesColor { get; set; } = null!;

    public string LipsColor { get; set; } = null!;

    public int UserId { get; set; }

    public virtual Avoid? Avoid { get; set; }

    public virtual Palette? Palette { get; set; }

    public virtual Suggested? Suggested { get; set; }

    public virtual UserAccount User { get; set; } = null!;
}
