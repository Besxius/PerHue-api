using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class TestRequest
{
    public int Id { get; set; }

    public string? HairColor { get; set; }

    public string? EyesColor { get; set; }

    public string? LipsColor { get; set; }

    public string? SkinColor { get; set; }

    public string? Status { get; set; }

    public DateTime? Date { get; set; }

    public bool IsAitest { get; set; }

    public int UserAccountId { get; set; }

    public virtual ICollection<Aipicture> Aipictures { get; set; } = new List<Aipicture>();

    public virtual AitestResult? AitestResult { get; set; }

    public virtual ICollection<Picture> Pictures { get; set; } = new List<Picture>();

    public virtual ICollection<TestResponse> TestResponses { get; set; } = new List<TestResponse>();

    public virtual UserAccount UserAccount { get; set; } = null!;

    public virtual ICollection<Expert> Experts { get; set; } = new List<Expert>();
}
