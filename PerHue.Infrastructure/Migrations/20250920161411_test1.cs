using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerHue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__3214EC07E95320B5", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Color",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    HexCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Color__3214EC072E6AAEA1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColorType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ColorTyp__3214EC0720EADA05", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportTy__3214EC07C727F08C", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__3214EC0775E85296", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServicePackage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ServiceP__3214EC074CAFF8F5", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Topic__3214EC07D06F4F9C", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CapsulePalette",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CapsuleP__3214EC076251D5B8", x => x.Id);
                    table.ForeignKey(
                        name: "FKCapsulePal268315",
                        column: x => x.ColorTypeId,
                        principalTable: "ColorType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    Dob = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProfilePicture = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    IsAITested = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserAcco__3214EC075BFD24F7", x => x.Id);
                    table.ForeignKey(
                        name: "FKUserAccoun312334",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CapsulePalette_Color",
                columns: table => new
                {
                    CapsulePaletteId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CapsuleP__797C10454EFFDC8E", x => new { x.CapsulePaletteId, x.ColorId });
                    table.ForeignKey(
                        name: "FKCapsulePal722223",
                        column: x => x.CapsulePaletteId,
                        principalTable: "CapsulePalette",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCapsulePal75933",
                        column: x => x.ColorId,
                        principalTable: "Color",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Slogan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Logo = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Website = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Brand__3214EC07C94BDFA0", x => x.Id);
                    table.ForeignKey(
                        name: "FKBrand133468",
                        column: x => x.Id,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatRoom",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Person1 = table.Column<int>(type: "int", nullable: false),
                    Person2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChatRoom__3214EC07109FD287", x => x.Id);
                    table.ForeignKey(
                        name: "FKChatRoom84229",
                        column: x => x.Person2,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKChatRoom84230",
                        column: x => x.Person1,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Expert",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nickname = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Specialization = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Bio = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    YearsOfExperience = table.Column<short>(type: "smallint", nullable: false),
                    Languages = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    Certification = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Introduction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacebookAccount = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    LinkedInAccount = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    InstagramAccount = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Expert__3214EC076CA50B66", x => x.Id);
                    table.ForeignKey(
                        name: "FKExpert610142",
                        column: x => x.Id,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Is_Read = table.Column<bool>(type: "bit", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime", nullable: false),
                    Receiver = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__3214EC07A7C0006D", x => x.Id);
                    table.ForeignKey(
                        name: "FKNotificati163299",
                        column: x => x.Receiver,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reaction = table.Column<int>(type: "int", nullable: false),
                    View = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post__3214EC07C0550A83", x => x.Id);
                    table.ForeignKey(
                        name: "FKPost883700",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKPost986101",
                        column: x => x.UserId,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RefreshT__3214EC076296A932", x => x.Id);
                    table.ForeignKey(
                        name: "FKRefreshTok433736",
                        column: x => x.UserAccountId,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Picture = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    SkinColor = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    HairColor = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    EyesColor = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    LipsColor = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Type = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ColorTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TestResu__3214EC078C548135", x => x.Id);
                    table.ForeignKey(
                        name: "FKTestResult1660",
                        column: x => x.ColorTypeId,
                        principalTable: "ColorType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKTestResult297045",
                        column: x => x.UserId,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserSubscription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Duration = table.Column<short>(type: "smallint", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ServicePackageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserSubs__3214EC07D15E2842", x => x.Id);
                    table.ForeignKey(
                        name: "FKUserSubscr443828",
                        column: x => x.ServicePackageId,
                        principalTable: "ServicePackage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKUserSubscr62907",
                        column: x => x.UserId,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VerifyInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Nickname = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Specialization = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Bio = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    YearsOfExperience = table.Column<short>(type: "smallint", nullable: false),
                    Languages = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Certification = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VerifyIn__3214EC07236A998D", x => x.Id);
                    table.ForeignKey(
                        name: "FKVerifyInfo301359",
                        column: x => x.Id,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Carousel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Picture = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Color = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    BrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Carousel__3214EC07C0B4D60E", x => x.Id);
                    table.ForeignKey(
                        name: "FKCarousel383250",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Sale = table.Column<decimal>(type: "decimal(1,1)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product__3214EC071DE2B3D5", x => x.Id);
                    table.ForeignKey(
                        name: "FKProduct365970",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKProduct960477",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reply",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Reaction = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    ReplyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reply__3214EC0730AE6FBC", x => x.Id);
                    table.ForeignKey(
                        name: "FKReply241689",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKReply96398",
                        column: x => x.ReplyId,
                        principalTable: "Reply",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Color_TestResult",
                columns: table => new
                {
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    TestResultId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Color_Te__E3830415516B0AF4", x => new { x.ColorId, x.TestResultId });
                    table.ForeignKey(
                        name: "FKColor_Test74218",
                        column: x => x.ColorId,
                        principalTable: "Color",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKColor_Test80884",
                        column: x => x.TestResultId,
                        principalTable: "TestResult",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Sender = table.Column<int>(type: "int", nullable: false),
                    ChatRoomId = table.Column<int>(type: "int", nullable: false),
                    TestResultId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Message__3214EC07B0CF5A22", x => x.Id);
                    table.ForeignKey(
                        name: "FKMessage169107",
                        column: x => x.Sender,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKMessage32066",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRoom",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKMessage712793",
                        column: x => x.TestResultId,
                        principalTable: "TestResult",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SimpleColor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hexcode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TestResultId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SimpleCo__3214EC0792E547B5", x => x.Id);
                    table.ForeignKey(
                        name: "FKSimpleColo694170",
                        column: x => x.TestResultId,
                        principalTable: "TestResult",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestResult_CapsulePalette",
                columns: table => new
                {
                    TestResultId = table.Column<int>(type: "int", nullable: false),
                    CapsulePaletteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TestResu__E95C53E446D81DAD", x => new { x.TestResultId, x.CapsulePaletteId });
                    table.ForeignKey(
                        name: "FKTestResult47353",
                        column: x => x.CapsulePaletteId,
                        principalTable: "CapsulePalette",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKTestResult848745",
                        column: x => x.TestResultId,
                        principalTable: "TestResult",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TransactionId = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserSubscriptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__3214EC07FCC5BA56", x => x.Id);
                    table.ForeignKey(
                        name: "FKPayment506916",
                        column: x => x.UserId,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKPayment766453",
                        column: x => x.UserSubscriptionId,
                        principalTable: "UserSubscription",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductPicture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Picture = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductP__3214EC0779401686", x => x.Id);
                    table.ForeignKey(
                        name: "FKProductPic772500",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReportTypeId = table.Column<int>(type: "int", nullable: false),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
                    ReplyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Report__3214EC07819B64A3", x => x.Id);
                    table.ForeignKey(
                        name: "FKReport36488",
                        column: x => x.UserAccountId,
                        principalTable: "UserAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKReport370169",
                        column: x => x.ReplyId,
                        principalTable: "Reply",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKReport882469",
                        column: x => x.ReportTypeId,
                        principalTable: "ReportType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    OldStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    NewStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Mesage = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Metadata = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentL__3214EC07C4F0B962", x => x.Id);
                    table.ForeignKey(
                        name: "FKPaymentLog793082",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapsulePalette_ColorTypeId",
                table: "CapsulePalette",
                column: "ColorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CapsulePalette_Color_ColorId",
                table: "CapsulePalette_Color",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Carousel_BrandId",
                table: "Carousel",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoom_Person1",
                table: "ChatRoom",
                column: "Person1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoom_Person2",
                table: "ChatRoom",
                column: "Person2");

            migrationBuilder.CreateIndex(
                name: "UQ__Color__737584F69EE4A9BF",
                table: "Color",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Color__A7CAA840B41ADCCC",
                table: "Color",
                column: "HexCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Color_TestResult_TestResultId",
                table: "Color_TestResult",
                column: "TestResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ChatRoomId",
                table: "Message",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_Sender",
                table: "Message",
                column: "Sender");

            migrationBuilder.CreateIndex(
                name: "IX_Message_TestResultId",
                table: "Message",
                column: "TestResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Receiver",
                table: "Notification",
                column: "Receiver");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_UserId",
                table: "Payment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_UserSubscriptionId",
                table: "Payment",
                column: "UserSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLog_PaymentId",
                table: "PaymentLog",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_TopicId",
                table: "Post",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserId",
                table: "Post",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_BrandId",
                table: "Product",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPicture_ProductId",
                table: "ProductPicture",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserAccountId",
                table: "RefreshToken",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_PostId",
                table: "Reply",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_ReplyId",
                table: "Reply",
                column: "ReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReplyId",
                table: "Report",
                column: "ReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportTypeId",
                table: "Report",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_UserAccountId",
                table: "Report",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SimpleColor_TestResultId",
                table: "SimpleColor",
                column: "TestResultId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_ColorTypeId",
                table: "TestResult",
                column: "ColorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_UserId",
                table: "TestResult",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_CapsulePalette_CapsulePaletteId",
                table: "TestResult_CapsulePalette",
                column: "CapsulePaletteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_RoleId",
                table: "UserAccount",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscription_ServicePackageId",
                table: "UserSubscription",
                column: "ServicePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscription_UserId",
                table: "UserSubscription",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapsulePalette_Color");

            migrationBuilder.DropTable(
                name: "Carousel");

            migrationBuilder.DropTable(
                name: "Color_TestResult");

            migrationBuilder.DropTable(
                name: "Expert");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PaymentLog");

            migrationBuilder.DropTable(
                name: "ProductPicture");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "SimpleColor");

            migrationBuilder.DropTable(
                name: "TestResult_CapsulePalette");

            migrationBuilder.DropTable(
                name: "VerifyInformation");

            migrationBuilder.DropTable(
                name: "Color");

            migrationBuilder.DropTable(
                name: "ChatRoom");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Reply");

            migrationBuilder.DropTable(
                name: "ReportType");

            migrationBuilder.DropTable(
                name: "CapsulePalette");

            migrationBuilder.DropTable(
                name: "TestResult");

            migrationBuilder.DropTable(
                name: "UserSubscription");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "ColorType");

            migrationBuilder.DropTable(
                name: "ServicePackage");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "UserAccount");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
