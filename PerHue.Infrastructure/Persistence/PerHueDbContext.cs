using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;

namespace PerHue.Infrastructure.Persistence;

public partial class PerHueDbContext : DbContext
{
    public PerHueDbContext()
    {
    }

    public PerHueDbContext(DbContextOptions<PerHueDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AiPicture> AiPictures { get; set; }

    public virtual DbSet<AiTestResult> AiTestResults { get; set; }

    public virtual DbSet<CapsulePalette> CapsulePalettes { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<ColorType> ColorTypes { get; set; }

    public virtual DbSet<Expert> Experts { get; set; }

    public virtual DbSet<ExpertTestRequest> ExpertTestRequests { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentLog> PaymentLogs { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<TestRequest> TestRequests { get; set; }

    public virtual DbSet<TestResponse> TestResponses { get; set; }

    public virtual DbSet<TestResult> TestResults { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    public virtual DbSet<VerifyInformation> VerifyInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiPicture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AiPictur__3214EC0725F3DD92");

            entity.ToTable("AiPicture");

            entity.Property(e => e.Source).IsUnicode(false);

            entity.HasOne(d => d.TestRequest).WithMany(p => p.AiPictures)
                .HasForeignKey(d => d.TestRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAiPicture425173");
        });

        modelBuilder.Entity<AiTestResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AiTestRe__3214EC0721DE7536");

            entity.ToTable("AiTestResult");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AvoidedColor).IsUnicode(false);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.SuggestedColor).IsUnicode(false);

            entity.HasOne(d => d.ColorType).WithMany(p => p.AiTestResults)
                .HasForeignKey(d => d.ColorTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAiTestResu112231");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.AiTestResult)
                .HasForeignKey<AiTestResult>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAiTestResu885671");
        });

        modelBuilder.Entity<CapsulePalette>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CapsuleP__3214EC07693F60B7");

            entity.ToTable("CapsulePalette");

            entity.HasOne(d => d.ColorType).WithMany(p => p.CapsulePalettes)
                .HasForeignKey(d => d.ColorTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKCapsulePal268315");

            entity.HasMany(d => d.Colors).WithMany(p => p.CapsulePalettes)
                .UsingEntity<Dictionary<string, object>>(
                    "CapsulePaletteColor",
                    r => r.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKCapsulePal75933"),
                    l => l.HasOne<CapsulePalette>().WithMany()
                        .HasForeignKey("CapsulePaletteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKCapsulePal722223"),
                    j =>
                    {
                        j.HasKey("CapsulePaletteId", "ColorId").HasName("PK__CapsuleP__797C1045CFEEC762");
                        j.ToTable("CapsulePalette_Color");
                    });
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Color__3214EC07B647152C");

            entity.ToTable("Color");

            entity.HasIndex(e => e.Name, "UQ__Color__737584F6CAB998C3").IsUnique();

            entity.HasIndex(e => e.HexCode, "UQ__Color__A7CAA840873894CF").IsUnique();

            entity.Property(e => e.HexCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasMany(d => d.TestResults).WithMany(p => p.Colors)
                .UsingEntity<Dictionary<string, object>>(
                    "ColorTestResult",
                    r => r.HasOne<TestResult>().WithMany()
                        .HasForeignKey("TestResultId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKColor_Test80884"),
                    l => l.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKColor_Test74218"),
                    j =>
                    {
                        j.HasKey("ColorId", "TestResultId").HasName("PK__Color_Te__E3830415C20D1112");
                        j.ToTable("Color_TestResult");
                    });
        });

        modelBuilder.Entity<ColorType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ColorTyp__3214EC0724573752");

            entity.ToTable("ColorType");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Expert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Expert__3214EC07B4F7904E");

            entity.ToTable("Expert");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Bio).IsUnicode(false);
            entity.Property(e => e.Certification).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FacebookAccount).IsUnicode(false);
            entity.Property(e => e.InstagramAccount).IsUnicode(false);
            entity.Property(e => e.Languages)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LinkedInAccount).IsUnicode(false);
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Rating).HasColumnType("decimal(10, 3)");
            entity.Property(e => e.Specialization).IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Expert)
                .HasForeignKey<Expert>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKExpert610142");
        });

        modelBuilder.Entity<ExpertTestRequest>(entity =>
        {
            entity.HasKey(e => new { e.ExpertId, e.TestRequestId }).HasName("PK__Expert_T__0B386C707C8863A6");

            entity.ToTable("Expert_TestRequest");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Expert).WithMany(p => p.ExpertTestRequests)
                .HasForeignKey(d => d.ExpertId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKExpert_Tes360727");

            entity.HasOne(d => d.TestRequest).WithMany(p => p.ExpertTestRequests)
                .HasForeignKey(d => d.TestRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKExpert_Tes912400");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC076EAF7829");

            entity.ToTable("Notification");

            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.ReceivedTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.ReceiverNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.Receiver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKNotificati163299");

            entity.HasOne(d => d.TestRequest).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TestRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKNotificati150945");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC07338613F3");

            entity.ToTable("Payment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TransactionId).IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPayment451401");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPayment506916");
        });

        modelBuilder.Entity<PaymentLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentL__3214EC07AB6C4E41");

            entity.ToTable("PaymentLog");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EventType).IsUnicode(false);
            entity.Property(e => e.Mesage).IsUnicode(false);
            entity.Property(e => e.Metadata).IsUnicode(false);
            entity.Property(e => e.NewStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.OldStatus)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Payment).WithMany(p => p.PaymentLogs)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPaymentLog793082");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Photo__3214EC070FEC4D3F");

            entity.ToTable("Photo");

            entity.Property(e => e.PhotoUrl).IsUnicode(false);
            entity.Property(e => e.Type).HasMaxLength(255);

            entity.HasOne(d => d.VerifyInformation).WithMany(p => p.Photos)
                .HasForeignKey(d => d.VerifyInformationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPhoto777996");
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Picture__3214EC076E76F1D5");

            entity.ToTable("Picture");

            entity.Property(e => e.Source).IsUnicode(false);

            entity.HasOne(d => d.TestRequest).WithMany(p => p.Pictures)
                .HasForeignKey(d => d.TestRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPicture509691");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC077F355C3F");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.ExpireDate).HasColumnType("datetime");
            entity.Property(e => e.Token).IsUnicode(false);

            entity.HasOne(d => d.UserAccount).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKRefreshTok433736");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC07E299C602");

            entity.ToTable("Role");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceP__3214EC075B2305FA");

            entity.ToTable("ServicePackage");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TestRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TestRequ__3214EC07A82E208B");

            entity.ToTable("TestRequest");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EyesColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.HairColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LipsColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SkinColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TypeOfTest)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.UserAccount).WithMany(p => p.TestRequests)
                .HasForeignKey(d => d.UserAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTestReques386664");
        });

        modelBuilder.Entity<TestResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TestResp__3214EC07736EB119");

            entity.ToTable("TestResponse");

            entity.Property(e => e.BestColor).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.WorstColor).IsUnicode(false);

            entity.HasOne(d => d.ColorType).WithMany(p => p.TestResponses)
                .HasForeignKey(d => d.ColorTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTestRespon437540");

            entity.HasOne(d => d.Expert).WithMany(p => p.TestResponses)
                .HasForeignKey(d => d.ExpertId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTestRespon382802");

            entity.HasOne(d => d.TestRequest).WithMany(p => p.TestResponses)
                .HasForeignKey(d => d.TestRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTestRespon934475");
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TestResu__3214EC07D5A41EBE");

            entity.ToTable("TestResult");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Picture).IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.TestResults)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTestResult297045");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAcco__3214EC079AF661B8");

            entity.ToTable("UserAccount");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Fullname).HasMaxLength(255);
            entity.Property(e => e.IsAitested).HasColumnName("IsAITested");
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePicture).IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUserAccoun312334");
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSubs__3214EC07671CE2D8");

            entity.ToTable("UserSubscription");

            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.ServicePackage).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.ServicePackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUserSubscr443828");

            entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUserSubscr62907");
        });

        modelBuilder.Entity<VerifyInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VerifyIn__3214EC07CFE4D6F2");

            entity.ToTable("VerifyInformation");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Bio).IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Languages)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Specialization).IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.VerifyInformation)
                .HasForeignKey<VerifyInformation>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKVerifyInfo301359");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
