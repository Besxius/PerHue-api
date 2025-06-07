using Microsoft.OpenApi.Models;
using PerHue.Api.Utils;
using PerHue.Application.Extensions;
using PerHue.Infrastructure.Extensions;
using PerHue.Infrastructure.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(options =>
{
	options.LowercaseUrls = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Google OAuth API", Version = "v1" });

	// Tạo các định nghĩa cho OAuth2
	c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
	{
		Type = SecuritySchemeType.OAuth2,
		Flows = new OpenApiOAuthFlows
		{
			Implicit = new OpenApiOAuthFlow
			{
				AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
				Scopes = new Dictionary<string, string>
					{
						{ "email", "Access your email" }
					}
			}
		}
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = "oauth2"
					}
				},
				new List<string> { "email" }
			}
		});
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
				policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));

//builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
//builder.Services.AddProblemDetails();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
await seeder.Seed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Google OAuth API V1");
		c.OAuthClientId(builder.Configuration["Google:ClientId"]);  // Thêm Google Client ID của bạn
		c.OAuthClientSecret(builder.Configuration["Google:ClientSecret"]);  // Thêm Google Client Secret của bạn
		c.OAuthUsePkce();
	});
}

app.UseCors();
//app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
