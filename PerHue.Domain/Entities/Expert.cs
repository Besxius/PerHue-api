using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class Expert
{
    public int Id { get; set; }

    public string? Nickname { get; set; }

    public string Specialization { get; set; } = null!;

    public string Bio { get; set; } = null!;

    public short YearsOfExperience { get; set; }

    public string? Languages { get; set; }

    public decimal? Rating { get; set; }

    public string Certification { get; set; } = null!;

    public string? Introduction { get; set; }

    public string? FacebookAccount { get; set; }

    public string? LinkedInAccount { get; set; }

    public string? InstagramAccount { get; set; }

    public virtual UserAccount IdNavigation { get; set; } = null!;

    public virtual ICollection<TestResponse> TestResponses { get; set; } = new List<TestResponse>();

    public virtual ICollection<TestRequest> TestRequests { get; set; } = new List<TestRequest>();
}
