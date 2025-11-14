using Microsoft.AspNetCore.Authentication.JwtBearer;
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
			services.AddScoped<IVerificationRepository, VerificationRepository>();
			services.AddScoped<IExpertRepository, ExpertRepository>();
			services.AddScoped<INotificationRepository, NotificationRepository>();
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
			#endregion

			#region Other Services
			services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);
			services.AddScoped<Seeder>();
			services.AddScoped<JwtProvider>();
			services.AddScoped<PayOSPaymentService>();
			services.AddScoped<GeminiService>();
			services.AddScoped<EmailService>();
			services.AddScoped<IOtpService, OtpService>();
			services.AddScoped<IImageUploadService, CloudinaryService>();
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
			var jwtIssuer = configuration["Jwt:Issuer"];
			var jwtAudience = configuration["Jwt:Audience"];
			var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not found."));

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = jwtIssuer,
						ValidAudience = jwtAudience,
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ClockSkew = TimeSpan.Zero
					};
				});

			services.AddLogging(logging =>
			{
				logging.AddConsole();
				logging.SetMinimumLevel(LogLevel.Debug);
			});

			services.AddAuthorization();


			services.Configure<JwtSetting>(configuration.GetSection("Jwt"));
			#endregion

		}
	}
}
