using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class TestResult
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Picture { get; set; } = null!;

    public string? SkinColor { get; set; }

    public string? HairColor { get; set; }

    public string? EyesColor { get; set; }

    public string? LipsColor { get; set; }

    public string? Type { get; set; }

    public int ColorTypeId { get; set; }

    public virtual ColorType ColorType { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<SimpleColor> SimpleColors { get; set; } = new List<SimpleColor>();

    public virtual UserAccount User { get; set; } = null!;

    public virtual ICollection<CapsulePalette> CapsulePalettes { get; set; } = new List<CapsulePalette>();

    public virtual ICollection<Color> Colors { get; set; } = new List<Color>();
}
