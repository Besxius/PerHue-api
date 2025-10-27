using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class VerifyInformation
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string? Nickname { get; set; }

    public string IdentityPhoto { get; set; } = null!;

    public string Specialization { get; set; } = null!;

    public string Bio { get; set; } = null!;

    public short YearsOfExperience { get; set; }

    public string? Languages { get; set; }

    public string Certification { get; set; } = null!;

    public virtual UserAccount IdNavigation { get; set; } = null!;
}
