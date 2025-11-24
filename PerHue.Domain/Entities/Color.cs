using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Danh mục các màu sắc với tên và mã hex code duy nhất.
/// </summary>
public partial class Color
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string HexCode { get; set; } = null!;

    public virtual ICollection<CapsulePalette> CapsulePalettes { get; set; } = new List<CapsulePalette>();

    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
