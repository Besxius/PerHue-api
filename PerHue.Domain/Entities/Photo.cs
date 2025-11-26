using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Photo
{
    public int Id { get; set; }

    public string PhotoUrl { get; set; } = null!;

    public string? Type { get; set; }

    public int VerifyInformationId { get; set; }

    public virtual VerifyInformation VerifyInformation { get; set; } = null!;
}
