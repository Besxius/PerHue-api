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

    public virtual DbSet<Aipicture> Aipictures { get; set; }

    public virtual DbSet<AitestResult> AitestResults { get; set; }

    public virtual DbSet<AvoidedColor> AvoidedColors { get; set; }

    public virtual DbSet<CapsulePalette> CapsulePalettes { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<ColorType> ColorTypes { get; set; }

    public virtual DbSet<Expert> Experts { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentLog> PaymentLogs { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<SuggestedColor> SuggestedColors { get; set; }

    public virtual DbSet<TestRequest> TestRequests { get; set; }

    public virtual DbSet<TestResponse> TestResponses { get; set; }

    public virtual DbSet<TestResult> TestResults { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    public virtual DbSet<VerifyInformation> VerifyInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aipicture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AIPictur__3214EC07B4D4DE82");

            entity.ToTable("AIPicture");

            entity.Property(e => e.Source).IsUnicode(false);

            entity.HasOne(d => d.TestRequest).WithMany(p => p.Aipictures)
                .HasForeignKey(d => d.TestRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAIPicture780980");
        });

        modelBuilder.Entity<AitestResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AITestRe__3214EC07A3E8A3BB");

            entity.ToTable("AITestResult");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.ColorType).WithMany(p => p.AitestResults)
                .HasForeignKey(d => d.ColorTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAITestResu989051");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.AitestResult)
                .HasForeignKey<AitestResult>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAITestResu215611");
        });

        modelBuilder.Entity<AvoidedColor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AvoidedC__3214EC07356FDB27");

            entity.ToTable("AvoidedColor");

            entity.Property(e => e.AitestResultId).HasColumnName("AITestResultId");
            entity.Property(e => e.Hexcode)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.AitestResult).WithMany(p => p.AvoidedColors)
                .HasForeignKey(d => d.AitestResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAvoidedCol899809");
        });

        modelBuilder.Entity<CapsulePalette>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CapsuleP__3214EC07305431B6");

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
                        j.HasKey("CapsulePaletteId", "ColorId").HasName("PK__CapsuleP__797C1045AC3A2530");
                        j.ToTable("CapsulePalette_Color");
                    });
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Color__3214EC078844A777");

            entity.ToTable("Color");

            entity.HasIndex(e => e.Name, "UQ__Color__737584F691498704").IsUnique();

            entity.HasIndex(e => e.HexCode, "UQ__Color__A7CAA8402670F12A").IsUnique();

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
                        j.HasKey("ColorId", "TestResultId").HasName("PK__Color_Te__E3830415171E8D93");
                        j.ToTable("Color_TestResult");
                    });
        });

        modelBuilder.Entity<ColorType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ColorTyp__3214EC0794C0A0AB");

            entity.ToTable("ColorType");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Expert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Expert__3214EC071A626DC6");

            entity.ToTable("Expert");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Bio).IsUnicode(false);
            entity.Property(e => e.Certification).IsUnicode(false);
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

            entity.HasMany(d => d.TestRequests).WithMany(p => p.Experts)
                .UsingEntity<Dictionary<string, object>>(
                    "ExpertTestRequest",
                    r => r.HasOne<TestRequest>().WithMany()
                        .HasForeignKey("TestRequestId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKExpert_Tes912400"),
                    l => l.HasOne<Expert>().WithMany()
                        .HasForeignKey("ExpertId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKExpert_Tes360727"),
                    j =>
                    {
                        j.HasKey("ExpertId", "TestRequestId").HasName("PK__Expert_T__0B386C703026DC93");
                        j.ToTable("Expert_TestRequest");
                    });
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC0798270DD9");

            entity.ToTable("Notification");

            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.IsRead).HasColumnName("Is_Read");
            entity.Property(e => e.Time).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.ReceiverNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.Receiver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKNotificati163299");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC072DDC5159");

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
            entity.HasKey(e => e.Id).HasName("PK__PaymentL__3214EC07558F77D2");

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

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Picture__3214EC07FCEFBFDA");

            entity.ToTable("Picture");

            entity.Property(e => e.Source).IsUnicode(false);

            entity.HasOne(d => d.TestRequest).WithMany(p => p.Pictures)
                .HasForeignKey(d => d.TestRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPicture509691");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07D35C9CE3");

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
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC07176837AC");

            entity.ToTable("Role");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceP__3214EC07B6859930");

            entity.ToTable("ServicePackage");

            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SuggestedColor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Suggeste__3214EC07E2534D0A");

            entity.ToTable("SuggestedColor");

            entity.Property(e => e.AitestResultId).HasColumnName("AITestResultId");
            entity.Property(e => e.Hexcode)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.AitestResult).WithMany(p => p.SuggestedColors)
                .HasForeignKey(d => d.AitestResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSuggestedC879914");
        });

        modelBuilder.Entity<TestRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TestRequ__3214EC070FAA2EB0");

            entity.ToTable("TestRequest");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.EyesColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.HairColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsAitest).HasColumnName("IsAITest");
            entity.Property(e => e.LipsColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SkinColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.UserAccount).WithMany(p => p.TestRequests)
                .HasForeignKey(d => d.UserAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTestReques386664");
        });

        modelBuilder.Entity<TestResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TestResp__3214EC07271C5405");

            entity.ToTable("TestResponse");

            entity.Property(e => e.Date).HasColumnType("datetime");

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

            entity.HasMany(d => d.Colors).WithMany(p => p.TestResponses)
                .UsingEntity<Dictionary<string, object>>(
                    "BestColor",
                    r => r.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKBestColor780206"),
                    l => l.HasOne<TestResponse>().WithMany()
                        .HasForeignKey("TestResponseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKBestColor452939"),
                    j =>
                    {
                        j.HasKey("TestResponseId", "ColorId").HasName("PK__BestColo__89AA5D6EA5CCDCF5");
                        j.ToTable("BestColor");
                    });

            entity.HasMany(d => d.ColorsNavigation).WithMany(p => p.TestResponsesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "WorstColor",
                    r => r.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKWorstColor369937"),
                    l => l.HasOne<TestResponse>().WithMany()
                        .HasForeignKey("TestResponseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKWorstColor697204"),
                    j =>
                    {
                        j.HasKey("TestResponseId", "ColorId").HasName("PK__WorstCol__89AA5D6EF2894E70");
                        j.ToTable("WorstColor");
                    });
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TestResu__3214EC0774902CA8");

            entity.ToTable("TestResult");

            entity.Property(e => e.Picture).IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.TestResults)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTestResult297045");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAcco__3214EC073016685D");

            entity.ToTable("UserAccount");

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
            entity.HasKey(e => e.Id).HasName("PK__UserSubs__3214EC07858B9E8D");

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
            entity.HasKey(e => e.Id).HasName("PK__VerifyIn__3214EC072DBB02EA");

            entity.ToTable("VerifyInformation");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Bio).IsUnicode(false);
            entity.Property(e => e.Certification).IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Languages)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Picture).IsUnicode(false);
            entity.Property(e => e.Specialization).IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.VerifyInformation)
                .HasForeignKey<VerifyInformation>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKVerifyInfo301359");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
