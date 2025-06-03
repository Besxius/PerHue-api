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

    public virtual DbSet<AiTestResult> AiTestResults { get; set; }

    public virtual DbSet<Avoid> Avoids { get; set; }

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

    public virtual DbSet<Palette> Palettes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentLog> PaymentLogs { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductPicture> ProductPictures { get; set; }

    public virtual DbSet<Reply> Replies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<Suggested> Suggesteds { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    public virtual DbSet<VerifyInformation> VerifyInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiTestResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AiTestRe__3214EC07E008096E");

            entity.ToTable("AiTestResult");

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

            entity.HasOne(d => d.User).WithMany(p => p.AiTestResults)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAiTestResu563974");
        });

        modelBuilder.Entity<Avoid>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Avoid__3214EC077AB9F648");

            entity.ToTable("Avoid");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Avoid)
                .HasForeignKey<Avoid>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAvoid425219");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brand__3214EC07A9A160CB");

            entity.ToTable("Brand");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Link)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Logo).IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Slogan).HasMaxLength(255);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Brand)
                .HasForeignKey<Brand>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKBrand133468");
        });

        modelBuilder.Entity<CapsulePalette>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CapsuleP__3214EC07C549DF41");

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
                        j.HasKey("CapsulePaletteId", "ColorId").HasName("PK__CapsuleP__797C1045418EBF84");
                        j.ToTable("CapsulePalette_Color");
                    });
        });

        modelBuilder.Entity<Carousel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Carousel__3214EC073250DB39");

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
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC07E7CB51FE");

            entity.ToTable("Category");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatRoom__3214EC07C65AEC62");

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
            entity.HasKey(e => e.Id).HasName("PK__Color__3214EC07D5033BC7");

            entity.ToTable("Color");

            entity.Property(e => e.HexCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasMany(d => d.Avoids).WithMany(p => p.Colors)
                .UsingEntity<Dictionary<string, object>>(
                    "ColorAvoid",
                    r => r.HasOne<Avoid>().WithMany()
                        .HasForeignKey("AvoidId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKColor_Avoi495417"),
                    l => l.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKColor_Avoi166920"),
                    j =>
                    {
                        j.HasKey("ColorId", "AvoidId").HasName("PK__Color_Av__5AF28142100F8742");
                        j.ToTable("Color_Avoid");
                    });

            entity.HasMany(d => d.Suggesteds).WithMany(p => p.Colors)
                .UsingEntity<Dictionary<string, object>>(
                    "ColorSuggested",
                    r => r.HasOne<Suggested>().WithMany()
                        .HasForeignKey("SuggestedId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKColor_Sugg730898"),
                    l => l.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKColor_Sugg867674"),
                    j =>
                    {
                        j.HasKey("ColorId", "SuggestedId").HasName("PK__Color_Su__745BF8CCA8991C59");
                        j.ToTable("Color_Suggested");
                    });
        });

        modelBuilder.Entity<ColorType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ColorTyp__3214EC0795440294");

            entity.ToTable("ColorType");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Expert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Expert__3214EC07D707CEEA");

            entity.ToTable("Expert");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Bio).IsUnicode(false);
            entity.Property(e => e.Certification).IsUnicode(false);
            entity.Property(e => e.Languages)
                .HasMaxLength(50)
                .IsUnicode(false);
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
            entity.HasKey(e => e.Id).HasName("PK__Message__3214EC07C5B46DE3");

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
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07AF3F413B");

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

        modelBuilder.Entity<Palette>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Palette__3214EC07F7B51414");

            entity.ToTable("Palette");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Palette)
                .HasForeignKey<Palette>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPalette663780");

            entity.HasMany(d => d.CapsulePalettes).WithMany(p => p.Palettes)
                .UsingEntity<Dictionary<string, object>>(
                    "PaletteCapsulePalette",
                    r => r.HasOne<CapsulePalette>().WithMany()
                        .HasForeignKey("CapsulePaletteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKPalette_Ca168791"),
                    l => l.HasOne<Palette>().WithMany()
                        .HasForeignKey("PaletteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKPalette_Ca115545"),
                    j =>
                    {
                        j.HasKey("PaletteId", "CapsulePaletteId").HasName("PK__Palette___B98889A7657197EE");
                        j.ToTable("Palette_CapsulePalette");
                    });
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC07E677A25F");

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
            entity.HasKey(e => e.Id).HasName("PK__PaymentL__3214EC0710C02682");

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
            entity.HasKey(e => e.Id).HasName("PK__Post__3214EC0734FCDE48");

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
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC07E0A91D8E");

            entity.ToTable("Product");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sale).HasColumnType("decimal(1, 1)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProduct365970");

            entity.HasMany(d => d.Colors).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductColor",
                    r => r.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKProduct_Co57564"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKProduct_Co884415"),
                    j =>
                    {
                        j.HasKey("ProductId", "ColorId").HasName("PK__Product___7CD6B0B9A4D7BE4B");
                        j.ToTable("Product_Color");
                    });
        });

        modelBuilder.Entity<ProductPicture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductP__3214EC07A42C2F30");

            entity.ToTable("ProductPicture");

            entity.Property(e => e.Picture).IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPictures)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProductPic772500");
        });

        modelBuilder.Entity<Reply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reply__3214EC0775A1219D");

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
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC07F7E3AC50");

            entity.ToTable("Role");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceP__3214EC0745CFD2B0");

            entity.ToTable("ServicePackage");

            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Suggested>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Suggeste__3214EC070F3CC9D8");

            entity.ToTable("Suggested");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Suggested)
                .HasForeignKey<Suggested>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSuggested621498");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Topic__3214EC0768BDE62C");

            entity.ToTable("Topic");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAcco__3214EC07BCBB2C86");

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
            entity.HasKey(e => e.Id).HasName("PK__UserSubs__3214EC072FCA375E");

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
            entity.HasKey(e => e.Id).HasName("PK__VerifyIn__3214EC07F85BA787");

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
