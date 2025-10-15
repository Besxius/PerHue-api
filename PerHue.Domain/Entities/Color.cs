using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Color
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string HexCode { get; set; } = null!;

    public virtual ICollection<CapsulePalette> CapsulePalettes { get; set; } = new List<CapsulePalette>();

    public virtual ICollection<TestResponse> TestResponses { get; set; } = new List<TestResponse>();

    public virtual ICollection<TestResponse> TestResponsesNavigation { get; set; } = new List<TestResponse>();

    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
