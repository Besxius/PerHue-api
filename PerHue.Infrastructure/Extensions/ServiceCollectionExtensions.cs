using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
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
			services.AddScoped<IColorRepository, ColorRepository>();
			services.AddScoped<ICapsulePaletteRepository, CapsulePaletteRepository>();
			services.AddScoped<IColorTypeRepository, ColorTypeRespository>();
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
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
			})
			.AddCookie(options =>
			{
				options.Cookie.SameSite = SameSiteMode.None; // Bảo vệ chống CSRF
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Đảm bảo cookie chỉ được gửi qua HTTPS
				options.Cookie.HttpOnly = false; // Ngăn chặn truy cập cookie từ JavaScript
			})
			.AddGoogle(options =>
			{
				options.ClientId = configuration["Google:ClientId"];
				options.ClientSecret = configuration["Google:ClientSecret"];
				options.Scope.Add("email");
				options.Scope.Add("profile");
				options.SaveTokens = true;
				options.CallbackPath = configuration["Google:CallbackPath"];
				options.Events.OnRedirectToAuthorizationEndpoint = context =>
				{
					context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
					return Task.CompletedTask;
				};
			})
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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
			services.AddDistributedMemoryCache();
			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = false;
				options.Cookie.IsEssential = true;
			});
			services.AddAuthorization();
			#endregion
		}
	}
}
