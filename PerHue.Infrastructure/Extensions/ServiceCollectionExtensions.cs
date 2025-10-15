using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Domain.IRepositories;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.AI;
using PerHue.Infrastructure.Authentication;
using PerHue.Infrastructure.Persistence;
using PerHue.Infrastructure.Repositories;
using PerHue.Infrastructure.Services;
using PerHue.Infrastructure.ServicesProviders;
using PerHue.Infrastructure.SignalR.BroadcastService;
using PerHue.Infrastructure.UnitOfWorks;
using PerHue.Infrastructure.Utils;
using System.Security.Claims;
using System.Text;

namespace PerHue.Infrastructure.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection");
			services.AddDbContext<PerHueDbContext>(options => options.UseSqlServer(connectionString));

			#region Repositories
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
			services.AddScoped<IPaymentRepository, PaymentRepository>();
			services.AddScoped<IServicePackageRepository, ServicePackageRepository>();
			services.AddScoped<IPaymentLogRepository, PaymentLogRepository>();
			services.AddScoped<IRoleRepository, RoleRepository>();
			services.AddScoped<IColorRepository, ColorRepository>();
			services.AddScoped<ICapsulePaletteRepository, CapsulePaletteRepository>();
			services.AddScoped<IColorTypeRepository, ColorTypeRespository>();
			services.AddScoped<ITestResultRepository, TestResultRepository>();
			services.AddScoped<ISimpleColorRepository, SimpleColorRepository>();
			services.AddScoped<IVerificationRepository, VerificationRepository>();
			services.AddScoped<IExpertRepository, ExpertRepository>();
			services.AddScoped<INotificationRepository, NotificationRepository>();
			services.AddScoped<IPostRepository, PostRepository>();
			#endregion

			#region Services
			services.AddScoped<IServicesProvider, ServicesProvider>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<IServicePackageService, ServicePackageService>();
			services.AddScoped<IPaymentLogService, PaymentLogService>();
			services.AddScoped<IRoleService, RoleService>();
			services.AddScoped<IColorService, ColorService>();
			services.AddScoped<ICapsulePaletteService, CapsulePaletteService>();
			services.AddScoped<IColorTypeService, ColorTypeService>();
			services.AddScoped<ITestResultService, TestResultService>();
			services.AddScoped<IVerificationService, VerificationService>();
			services.AddScoped<IExpertService, ExpertService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<IPostService, PostService>();
			#endregion

			#region Other Services
			services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);
			services.AddScoped<Seeder>();
			services.AddScoped<JwtProvider>();
			services.AddScoped<PayOSPaymentService>();
			services.AddScoped<GeminiService>();
			services.AddScoped<EmailService>();
			services.AddScoped<IOtpService,OtpService>();
			services.AddSingleton<RedisHelper>(sp => {
				var redisHost = configuration["Redis:Host"];
				var redisPort = configuration["Redis:Port"];
				var redisPassword = configuration["Redis:Password"];

				string connectionString = $"{redisHost}:{redisPort},password={redisPassword},abortConnect=false";
				Console.WriteLine($"Connecting to Redis Cloud at: {redisHost}:{redisPort}");
				return new RedisHelper(connectionString);
			});
			// SignalR-Automated Email and Package Expire Service
			services.AddSignalR();
			services.AddHostedService<AutoEmail>();
			services.AddHostedService<PackageExpire>();
			#endregion

			#region Authentication	
			var secretKey = configuration["AppSettings:SecretKey"];
			var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey!);
			services.AddAuthentication(options =>
			{
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
			})
			.AddCookie(options =>
			{
				// Cấu hình Cookie Authentication.
				// Đây là nơi thông tin phiên người dùng sẽ được lưu trữ.
				options.ExpireTimeSpan = TimeSpan.FromDays(30); // Thời gian sống của cookie xác thực
				options.SlidingExpiration = true; // Gia hạn thời gian sống của cookie khi có hoạt động
				options.Cookie.HttpOnly = true; // Ngăn JavaScript truy cập cookie (quan trọng cho bảo mật)
				options.Cookie.IsEssential = true; // Đánh dấu cookie là cần thiết cho chức năng ứng dụng
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Chỉ gửi cookie qua HTTPS
				options.Cookie.SameSite = SameSiteMode.None;
			})
			.AddGoogle(options =>
			{
				options.ClientId = configuration["Google:ClientId"];
				options.ClientSecret = configuration["Google:ClientSecret"];
				options.Scope.Add("email");
				options.SaveTokens = true;
				options.CallbackPath = configuration["Google:CallbackPath"];

				// Bạn có thể tùy chỉnh thông tin người dùng được lưu vào ClaimsPrincipal tại đây
				// Ví dụ: thêm các claims tùy chỉnh từ thông tin Google trả về
				options.Events.OnCreatingTicket = context =>
				{
					// Lấy thông tin email và tên từ Google
					var email = context.Identity.FindFirst(ClaimTypes.Email)?.Value;
					var name = context.Identity.FindFirst(ClaimTypes.Name)?.Value;
					var nameIdentifier = context.Identity.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Google User ID

					// Bạn có thể thêm hoặc sửa đổi các claims trong context.Identity
					// Ví dụ: thêm một claim tùy chỉnh
					// context.Identity.AddClaim(new Claim("CustomClaimType", "CustomValue"));

					// Nếu bạn muốn lưu trữ thêm thông tin từ Google (ví dụ: access_token của Google)
					// bạn có thể lưu nó vào AuthenticationProperties
					// context.Properties.StoreTokens(context.Tokens); // SaveTokens = true đã làm điều này

					return Task.CompletedTask;
				};
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = false,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
					ClockSkew = TimeSpan.Zero
				};
			});
			;

			services.AddLogging(logging =>
			{
				logging.AddConsole();
				logging.SetMinimumLevel(LogLevel.Debug);
			}); 

			services.Configure<AppSetting>(configuration.GetSection("AppSettings"));

			

		/*	services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = false,

					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

					ClockSkew = TimeSpan.Zero
				};
			});*/

			#endregion

		}
	}
}
