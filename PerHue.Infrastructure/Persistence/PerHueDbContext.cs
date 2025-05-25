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

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductPicture> ProductPictures { get; set; }

    public virtual DbSet<Reply> Replies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<Suggested> Suggesteds { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<VerifyInformation> VerifyInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiTestResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ai_test_result_pkey");

            entity.ToTable("ai_test_result");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EyesColor)
                .HasMaxLength(20)
                .HasColumnName("eyes_color");
            entity.Property(e => e.HairColor)
                .HasMaxLength(20)
                .HasColumnName("hair_color");
            entity.Property(e => e.LipsColor)
                .HasMaxLength(20)
                .HasColumnName("lips_color");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .HasColumnName("picture");
            entity.Property(e => e.SkinColor)
                .HasMaxLength(20)
                .HasColumnName("skin_color");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AiTestResults)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkai_test_re190647");
        });

        modelBuilder.Entity<Avoid>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("avoid_pkey");

            entity.ToTable("avoid");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Avoid)
                .HasForeignKey<Avoid>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkavoid989782");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("brand_pkey");

            entity.ToTable("brand");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Link)
                .HasMaxLength(100)
                .HasColumnName("link");
            entity.Property(e => e.Logo)
                .HasMaxLength(255)
                .HasColumnName("logo");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.Slogan)
                .HasMaxLength(100)
                .HasColumnName("slogan");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Brand)
                .HasForeignKey<Brand>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkbrand213295");
        });

        modelBuilder.Entity<CapsulePalette>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("capsule_palette_pkey");

            entity.ToTable("capsule_palette");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ColorTypeId).HasColumnName("color_type_id");

            entity.HasOne(d => d.ColorType).WithMany(p => p.CapsulePalettes)
                .HasForeignKey(d => d.ColorTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkcapsule_pa2677");

            entity.HasMany(d => d.Colors).WithMany(p => p.CapsulePalettes)
                .UsingEntity<Dictionary<string, object>>(
                    "CapsulePaletteColor",
                    r => r.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fkcapsule_pa182676"),
                    l => l.HasOne<CapsulePalette>().WithMany()
                        .HasForeignKey("CapsulePaletteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fkcapsule_pa163002"),
                    j =>
                    {
                        j.HasKey("CapsulePaletteId", "ColorId").HasName("capsule_palette_color_pkey");
                        j.ToTable("capsule_palette_color");
                        j.IndexerProperty<int>("CapsulePaletteId").HasColumnName("capsule_palette_id");
                        j.IndexerProperty<int>("ColorId").HasColumnName("color_id");
                    });
        });

        modelBuilder.Entity<Carousel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("carousel_pkey");

            entity.ToTable("carousel");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.Color)
                .HasMaxLength(10)
                .HasColumnName("color");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .HasColumnName("picture");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");

            entity.HasOne(d => d.Brand).WithMany(p => p.Carousels)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkcarousel257824");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chat_room_pkey");

            entity.ToTable("chat_room");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Person1).HasColumnName("person1");
            entity.Property(e => e.Person2).HasColumnName("person2");

            entity.HasOne(d => d.Person1Navigation).WithMany(p => p.ChatRoomPerson1Navigations)
                .HasForeignKey(d => d.Person1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkchat_room351897");

            entity.HasOne(d => d.Person2Navigation).WithMany(p => p.ChatRoomPerson2Navigations)
                .HasForeignKey(d => d.Person2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkchat_room351896");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("color_pkey");

            entity.ToTable("color");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.HexCode)
                .HasMaxLength(20)
                .HasColumnName("hex_code");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");

            entity.HasMany(d => d.Avoids).WithMany(p => p.Colors)
                .UsingEntity<Dictionary<string, object>>(
                    "ColorAvoid",
                    r => r.HasOne<Avoid>().WithMany()
                        .HasForeignKey("AvoidId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fkcolor_avoi983755"),
                    l => l.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fkcolor_avoi217577"),
                    j =>
                    {
                        j.HasKey("ColorId", "AvoidId").HasName("color_avoid_pkey");
                        j.ToTable("color_avoid");
                        j.IndexerProperty<int>("ColorId").HasColumnName("color_id");
                        j.IndexerProperty<int>("AvoidId").HasColumnName("avoid_id");
                    });

            entity.HasMany(d => d.Suggesteds).WithMany(p => p.Colors)
                .UsingEntity<Dictionary<string, object>>(
                    "ColorSuggested",
                    r => r.HasOne<Suggested>().WithMany()
                        .HasForeignKey("SuggestedId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fkcolor_sugg317685"),
                    l => l.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fkcolor_sugg280581"),
                    j =>
                    {
                        j.HasKey("ColorId", "SuggestedId").HasName("color_suggested_pkey");
                        j.ToTable("color_suggested");
                        j.IndexerProperty<int>("ColorId").HasColumnName("color_id");
                        j.IndexerProperty<int>("SuggestedId").HasColumnName("suggested_id");
                    });
        });

        modelBuilder.Entity<ColorType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("color_type_pkey");

            entity.ToTable("color_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Expert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("expert_pkey");

            entity.ToTable("expert");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Bio)
                .HasMaxLength(255)
                .HasColumnName("bio");
            entity.Property(e => e.Certification)
                .HasMaxLength(255)
                .HasColumnName("certification");
            entity.Property(e => e.Languages)
                .HasMaxLength(50)
                .HasColumnName("languages");
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .HasColumnName("nickname");
            entity.Property(e => e.Rating)
                .HasPrecision(10, 3)
                .HasColumnName("rating");
            entity.Property(e => e.Specialization)
                .HasMaxLength(255)
                .HasColumnName("specialization");
            entity.Property(e => e.YearsOfExperience).HasColumnName("years_of_experience");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Expert)
                .HasForeignKey<Expert>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkexpert985314");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("message_pkey");

            entity.ToTable("message");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChatRoomId).HasColumnName("chat_room_id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.Sender).HasColumnName("sender");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
            entity.Property(e => e.Time)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkmessage521334");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.Sender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkmessage177656");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_pkey");

            entity.ToTable("notification");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Receiver).HasColumnName("receiver");
            entity.Property(e => e.Time)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");

            entity.HasOne(d => d.ReceiverNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.Receiver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fknotificati183464");
        });

        modelBuilder.Entity<Palette>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("palette_pkey");

            entity.ToTable("palette");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Palette)
                .HasForeignKey<Palette>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkpalette892807");

            entity.HasMany(d => d.CapsulePalettes).WithMany(p => p.Palettes)
                .UsingEntity<Dictionary<string, object>>(
                    "PaletteCapsulePalette",
                    r => r.HasOne<CapsulePalette>().WithMany()
                        .HasForeignKey("CapsulePaletteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fkpalette_ca603434"),
                    l => l.HasOne<Palette>().WithMany()
                        .HasForeignKey("PaletteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fkpalette_ca371988"),
                    j =>
                    {
                        j.HasKey("PaletteId", "CapsulePaletteId").HasName("palette_capsule_palette_pkey");
                        j.ToTable("palette_capsule_palette");
                        j.IndexerProperty<int>("PaletteId").HasColumnName("palette_id");
                        j.IndexerProperty<int>("CapsulePaletteId").HasColumnName("capsule_palette_id");
                    });
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_pkey");

            entity.ToTable("payment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Message)
                .HasMaxLength(50)
                .HasColumnName("message");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Time)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkpayment832702");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("post_pkey");

            entity.ToTable("post");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.Reaction).HasColumnName("reaction");
            entity.Property(e => e.Time)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");
            entity.Property(e => e.TopicId).HasColumnName("topic_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Topic).WithMany(p => p.Posts)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkpost160116");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkpost646482");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_pkey");

            entity.ToTable("product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Sale)
                .HasPrecision(1, 3)
                .HasColumnName("sale");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkproduct18273");

            entity.HasMany(d => d.Colors).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductColor",
                    r => r.HasOne<Color>().WithMany()
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fkproduct_co644657"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fkproduct_co749051"),
                    j =>
                    {
                        j.HasKey("ProductId", "ColorId").HasName("product_color_pkey");
                        j.ToTable("product_color");
                        j.IndexerProperty<int>("ProductId").HasColumnName("product_id");
                        j.IndexerProperty<int>("ColorId").HasColumnName("color_id");
                    });
        });

        modelBuilder.Entity<ProductPicture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_picture_pkey");

            entity.ToTable("product_picture");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .HasColumnName("picture");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPictures)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkproduct_pi17947");
        });

        modelBuilder.Entity<Reply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reply_pkey");

            entity.ToTable("reply");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Reaction).HasColumnName("reaction");
            entity.Property(e => e.ReplyId).HasColumnName("reply_id");

            entity.HasOne(d => d.Post).WithMany(p => p.Replies)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkreply786601");

            entity.HasOne(d => d.ReplyNavigation).WithMany(p => p.InverseReplyNavigation)
                .HasForeignKey(d => d.ReplyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkreply278896");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("service_package_pkey");

            entity.ToTable("service_package");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
        });

        modelBuilder.Entity<Suggested>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("suggested_pkey");

            entity.ToTable("suggested");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Suggested)
                .HasForeignKey<Suggested>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fksuggested935089");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("topic_pkey");

            entity.ToTable("topic");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_account_pkey");

            entity.ToTable("user_account");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsAiTested).HasColumnName("is_ai_tested");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .HasColumnName("phone");
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(255)
                .HasColumnName("profile_picture");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.ServicePackageId).HasColumnName("service_package_id");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Username)
                .HasMaxLength(10)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkuser_accou431404");

            entity.HasOne(d => d.ServicePackage).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.ServicePackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkuser_accou392006");
        });

        modelBuilder.Entity<VerifyInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("verify_information_pkey");

            entity.ToTable("verify_information");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Bio)
                .HasMaxLength(255)
                .HasColumnName("bio");
            entity.Property(e => e.Certification)
                .HasMaxLength(255)
                .HasColumnName("certification");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Languages)
                .HasMaxLength(50)
                .HasColumnName("languages");
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .HasColumnName("nickname");
            entity.Property(e => e.Specialization)
                .HasMaxLength(255)
                .HasColumnName("specialization");
            entity.Property(e => e.YearsOfExperience).HasColumnName("years_of_experience");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.VerifyInformation)
                .HasForeignKey<VerifyInformation>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkverify_inf617092");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
