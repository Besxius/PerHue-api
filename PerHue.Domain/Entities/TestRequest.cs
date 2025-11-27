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

    public DateTime? CreatedDate { get; set; }

    public string TypeOfTest { get; set; } = null!;

    public int UserAccountId { get; set; }

    public virtual ICollection<AiPicture> AiPictures { get; set; } = new List<AiPicture>();

    public virtual AiTestResult? AiTestResult { get; set; }

    public virtual ICollection<ExpertTestRequest> ExpertTestRequests { get; set; } = new List<ExpertTestRequest>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Picture> Pictures { get; set; } = new List<Picture>();

    public virtual ICollection<TestResponse> TestResponses { get; set; } = new List<TestResponse>();

    public virtual UserAccount UserAccount { get; set; } = null!;
}
