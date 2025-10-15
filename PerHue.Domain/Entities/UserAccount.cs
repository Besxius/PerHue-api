using System;
using System.Collections.Generic;

namespace PerHue.Domain.Entities;

public partial class UserAccount
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Fullname { get; set; }

    public string? Phone { get; set; }

    public bool Gender { get; set; }

    public DateOnly? Dob { get; set; }

    public bool IsActive { get; set; }

    public string? ProfilePicture { get; set; }

    public bool IsAitested { get; set; }

    public int RoleId { get; set; }

    public virtual Expert? Expert { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TestRequest> TestRequests { get; set; } = new List<TestRequest>();

    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();

    public virtual VerifyInformation? VerifyInformation { get; set; }
}
