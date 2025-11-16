using Microsoft.OpenApi.Models;
using PerHue.Application.Extensions;
using PerHue.Infrastructure.Extensions;
using PerHue.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(options =>
{
	options.LowercaseUrls = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerGen(); 
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo { Title = "App.API", Version = "v1" });

	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Vui lòng nhập 'Bearer ' theo sau là JWT token của bạn (ví dụ: Bearer eyJhb...).",
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
        }
	});
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
				policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));

var perhueAllowSpecificOrigins = "_perhueAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: perhueAllowSpecificOrigins,
		policy =>
		{
			policy
			.WithOrigins(
				"http://localhost:5009",
				"https://localhost:7092",
				"http://localhost:3333",
				"http://10.0.2.2:5009",
				"https://10.0.2.2:7092",
				"https://perhue16-b4hadyg9c5avfsa5.southeastasia-01.azurewebsites.net"
				)
			//.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials(); // RẤT QUAN TRỌNG: Cho phép gửi cookie, header Authorization, v.v.
								 // Nếu dùng AllowCredentials(), KHÔNG ĐƯỢC dùng AllowAnyOrigin().
		});
});

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
await seeder.Seed();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.UseExceptionHandler();
app.UseCors(perhueAllowSpecificOrigins);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<PerHue.Infrastructure.SignalR.Hub.ServerHub>("/serverhub");
app.MapControllers();

app.Run();
