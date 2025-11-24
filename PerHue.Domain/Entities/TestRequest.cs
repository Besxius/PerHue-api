using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

/// <summary>
/// Lưu yêu cầu test màu sắc từ người dùng, bao gồm thông tin màu tóc, mắt, môi, da, loại test (AI hoặc chuyên gia), và trạng thái xử lý.
/// </summary>
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
