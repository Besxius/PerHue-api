using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Tạo bộ sưu tập màu capsule (bảng màu tối giản) cho từng loại màu.
/// </summary>
public partial class CapsulePalette
{
    public int Id { get; set; }

    public int ColorTypeId { get; set; }

    public virtual ColorType ColorType { get; set; } = null!;

    public virtual ICollection<Color> Colors { get; set; } = new List<Color>();
}
