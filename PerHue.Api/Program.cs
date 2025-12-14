using Microsoft.OpenApi.Models;
using PerHue.Application.Extensions;
using PerHue.Domain.Entities;
using PerHue.Infrastructure.Extensions;
using PerHue.Infrastructure.Services;
using PerHue.Infrastructure.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(options =>
{
	options.LowercaseUrls = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var buildInfo = BuildInfoProvider.Load();
builder.Services.AddSwaggerGen(); 
builder.Services.AddSwaggerGen(options =>
{
	var description = buildInfo == null
	   ? "Build info not available"
	   : $"""
        **Last build:** {buildInfo.BuildTime:yyyy-MM-dd HH:mm:ss} UTC  
        **Branch:** {buildInfo.Branch}  
        **Commit:** `{buildInfo.Commit[..7]}`  
        **Run:** #{buildInfo.RunId}
        """;

	options.SwaggerDoc("v1", new OpenApiInfo { Title = "App.API", Version = "v1", Description = description });

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
app.UseCors();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<PerHue.Infrastructure.SignalR.Hub.ServerHub>("/serverhub");
app.MapControllers();
app.MapGet("/build-info", () =>
{
	var info = BuildInfoProvider.Load();
	return info is null ? Results.NotFound() : Results.Ok(info);
});

app.Run();
