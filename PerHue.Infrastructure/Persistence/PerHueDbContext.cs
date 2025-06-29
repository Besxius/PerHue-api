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

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<CapsulePalette> CapsulePalettes { get; set; }

    public virtual DbSet<Carousel> Carousels { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<ColorType> ColorTypes { get; set; }

    public virtual DbSet<Expert> Experts { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentLog> PaymentLogs { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductPicture> ProductPictures { get; set; }

    public virtual DbSet<Reply> Replies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<SimpleColor> SimpleColors { get; set; }

    public virtual DbSet<TestResult> TestResults { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    public virtual DbSet<VerifyInformation> VerifyInformations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:trongle16.database.windows.net,1433;Initial Catalog=PerHueDb;Persist Security Info=False;User ID=tl-admin;Password=trongle123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brand__3214EC079CF4B76B");

            entity.ToTable("Brand");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Logo).IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Slogan).HasMaxLength(255);
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Brand)
                .HasForeignKey<Brand>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKBrand133468");
        });

        modelBuilder.Entity<CapsulePalette>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CapsuleP__3214EC07E41190A4");

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
                        j.HasKey("CapsulePaletteId", "ColorId").HasName("PK__CapsuleP__797C1045B7135F86");
                        j.ToTable("CapsulePalette_Color");
                    });
        });

        modelBuilder.Entity<Carousel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Carousel__3214EC07E3A651AF");

            entity.ToTable("Carousel");

            entity.Property(e => e.Color)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Picture).IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Brand).WithMany(p => p.Carousels)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKCarousel383250");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC073A2F02DE");

            entity.ToTable("Category");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatRoom__3214EC07E0A552F4");

            entity.ToTable("ChatRoom");

            entity.HasOne(d => d.Person1Navigation).WithMany(p => p.ChatRoomPerson1Navigations)
                .HasForeignKey(d => d.Person1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKChatRoom84230");

            entity.HasOne(d => d.Person2Navigation).WithMany(p => p.ChatRoomPerson2Navigations)
                .HasForeignKey(d => d.Person2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKChatRoom84229");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Color__3214EC07A5AD6AA7");

            entity.ToTable("Color");

            entity.HasIndex(e => e.Name, "UQ__Color__737584F6AE855D6E").IsUnique();

            entity.HasIndex(e => e.HexCode, "UQ__Color__A7CAA84029C8ED92").IsUnique();

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
                        j.HasKey("ColorId", "TestResultId").HasName("PK__Color_Te__E38304154D272126");
                        j.ToTable("Color_TestResult");
                    });
        });

        modelBuilder.Entity<ColorType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ColorTyp__3214EC07D36A741D");

            entity.ToTable("ColorType");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Expert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Expert__3214EC0797BDCB5F");

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
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Message__3214EC074C2E6A51");

            entity.ToTable("Message");

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Time).HasColumnType("datetime");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKMessage32066");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.Sender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKMessage169107");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC071AD03EE5");

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
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC0754DDCD96");

            entity.ToTable("Payment");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TransactionId).IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPayment506916");

            entity.HasOne(d => d.UserSubscription).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserSubscriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPayment766453");
        });

        modelBuilder.Entity<PaymentLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentL__3214EC075616D8AE");

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

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Post__3214EC0730B949D7");

            entity.ToTable("Post");

            entity.Property(e => e.Time).HasColumnType("datetime");

            entity.HasOne(d => d.Topic).WithMany(p => p.Posts)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPost883700");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPost986101");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC0722E61777");

            entity.ToTable("Product");

            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sale).HasColumnType("decimal(1, 1)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProduct960477");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProduct365970");
        });

        modelBuilder.Entity<ProductPicture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductP__3214EC070A615BEB");

            entity.ToTable("ProductPicture");

            entity.Property(e => e.Picture).IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPictures)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProductPic772500");
        });

        modelBuilder.Entity<Reply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reply__3214EC07F84DA2F6");

            entity.ToTable("Reply");

            entity.Property(e => e.Content).IsUnicode(false);

            entity.HasOne(d => d.Post).WithMany(p => p.Replies)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKReply241689");

            entity.HasOne(d => d.ReplyNavigation).WithMany(p => p.InverseReplyNavigation)
                .HasForeignKey(d => d.ReplyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKReply96398");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC07712A5E19");

            entity.ToTable("Role");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceP__3214EC07875C672F");

            entity.ToTable("ServicePackage");

            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SimpleColor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SimpleCo__3214EC0754ED3379");

            entity.ToTable("SimpleColor");

            entity.Property(e => e.Hexcode)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.TestResult).WithMany(p => p.SimpleColors)
                .HasForeignKey(d => d.TestResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSimpleColo694170");
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TestResu__3214EC07F06F872F");

            entity.ToTable("TestResult");

            entity.Property(e => e.EyesColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.HairColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LipsColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Picture).IsUnicode(false);
            entity.Property(e => e.SkinColor)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.ColorType).WithMany(p => p.TestResults)
                .HasForeignKey(d => d.ColorTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTestResult1660");

            entity.HasOne(d => d.User).WithMany(p => p.TestResults)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTestResult297045");

            entity.HasMany(d => d.CapsulePalettes).WithMany(p => p.TestResults)
                .UsingEntity<Dictionary<string, object>>(
                    "TestResultCapsulePalette",
                    r => r.HasOne<CapsulePalette>().WithMany()
                        .HasForeignKey("CapsulePaletteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKTestResult47353"),
                    l => l.HasOne<TestResult>().WithMany()
                        .HasForeignKey("TestResultId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKTestResult848745"),
                    j =>
                    {
                        j.HasKey("TestResultId", "CapsulePaletteId").HasName("PK__TestResu__E95C53E4C9C6F2B2");
                        j.ToTable("TestResult_CapsulePalette");
                    });
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Topic__3214EC0777633BB4");

            entity.ToTable("Topic");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAcco__3214EC07756312FF");

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
            entity.HasKey(e => e.Id).HasName("PK__UserSubs__3214EC0730264031");

            entity.ToTable("UserSubscription");

            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

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
            entity.HasKey(e => e.Id).HasName("PK__VerifyIn__3214EC07F31A170F");

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
