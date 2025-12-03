using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class VerifyInformation
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string? Nickname { get; set; }

    public string Specialization { get; set; } = null!;

    public string Bio { get; set; } = null!;

    public short YearsOfExperience { get; set; }

    public string? Languages { get; set; }

    public string? Requirement { get; set; }

    public string Status { get; set; } = null!;

    public string? FacebookAccount { get; set; }

    public string? LinkedInAccount { get; set; }

    public string? InstagramAccount { get; set; }

    public virtual UserAccount IdNavigation { get; set; } = null!;

    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
}
