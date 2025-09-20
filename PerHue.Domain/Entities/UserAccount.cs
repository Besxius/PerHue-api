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

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<ChatRoom> ChatRoomPerson1Navigations { get; set; } = new List<ChatRoom>();

    public virtual ICollection<ChatRoom> ChatRoomPerson2Navigations { get; set; } = new List<ChatRoom>();

    public virtual Expert? Expert { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();

    public virtual VerifyInformation? VerifyInformation { get; set; }
}
