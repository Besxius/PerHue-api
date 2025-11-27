using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class ColorType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<AiTestResult> AiTestResults { get; set; } = new List<AiTestResult>();

    public virtual ICollection<CapsulePalette> CapsulePalettes { get; set; } = new List<CapsulePalette>();

    public virtual ICollection<TestResponse> TestResponses { get; set; } = new List<TestResponse>();

    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
