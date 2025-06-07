using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Domain.IRepositories;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Authentication;
using PerHue.Infrastructure.Persistence;
using PerHue.Infrastructure.Repositories;
using PerHue.Infrastructure.Services;
using PerHue.Infrastructure.ServicesProviders;
using PerHue.Infrastructure.UnitOfWorks;
using PerHue.Infrastructure.Utils;
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
			#endregion

			#region Services
			services.AddScoped<IServicesProvider, ServicesProvider>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<IServicePackageService, ServicePackageService>();
			services.AddScoped<IPaymentLogService, PaymentLogService>();
			services.AddScoped<IRoleService, RoleService>();
			#endregion

			#region Other Services
			services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);
			services.AddScoped<Seeder>();
			services.AddScoped<JwtProvider>();
			services.AddScoped<PayOSPaymentService>();
			#endregion

			#region Authentication	
			services.Configure<AppSetting>(configuration.GetSection("AppSettings"));

			var secretKey = configuration["AppSettings:SecretKey"];
			var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey!);

			services.AddAuthentication(options =>
			{
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;  // Đây là default scheme cho Google
				options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;  // Scheme mặc định cho Google Authentication
			})
			.AddCookie()  // Cấu hình cookie cho phiên làm việc
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>  // Cấu hình JWT cho API
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					// Định cấu hình kiểm tra token
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = false,  // Có thể để `false` nếu không muốn kiểm tra thời gian sống của token
			
					// Kiểm tra khóa ký
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
			
					ClockSkew = TimeSpan.Zero  // Xóa độ trễ thời gian giữa máy chủ và client
				};
			})
			.AddGoogle(options =>
			{
				options.ClientId = configuration["Google:ClientId"];
				options.ClientSecret = configuration["Google:ClientSecret"];
				options.Scope.Add("email");  // Cấp quyền truy cập email
				options.SaveTokens = true;   // Lưu token
			});
			#endregion
		}
	}
}
