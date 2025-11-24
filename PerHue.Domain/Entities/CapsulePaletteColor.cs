using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Bảng trung gian liên kết màu sắc với bộ sưu tập capsule.
/// </summary>
public partial class CapsulePaletteColor
{
    public int CapsulePaletteId { get; set; }

    public int ColorId { get; set; }
}
