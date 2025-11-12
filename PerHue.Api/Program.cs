using Microsoft.OpenApi.Models;
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
var googleClientId = builder.Configuration["Google:ClientId"];
var googleClientSecret = builder.Configuration["Google:ClientSecret"];

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo { Title = "App.API", Version = "v1" });
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter token  into field",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "bearer",
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
			[]
		}
	});
});

builder.Services.AddHttpClient();	

builder.Services.AddAuthorization();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
				policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()));

// Định nghĩa tên chính sách CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; // Tên chính sách của bạn

// Thêm dịch vụ CORS vào DI container
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: MyAllowSpecificOrigins,
		policy =>
		{
			// Cho phép các request từ một hoặc nhiều origin cụ thể
			// Thay thế "http://localhost:3000" và "https://yourfrontend.com"
			// bằng địa chỉ chính xác của frontend của bạn.

			policy
			.WithOrigins(
				"http://localhost:7092",
				"https://localhost:7092",
				"http://10.0.2.2:7092",  // Android Emulator HTTP
				"https://10.0.2.2:7092", // Android Emulator HTTPS
				"https://perhue16-b4hadyg9c5avfsa5.southeastasia-01.azurewebsites.net"
				)
			//.AllowAnyOrigin() // Cho phép tất cả các origin (cẩn thận với việc này trong môi trường sản xuất)
			.AllowAnyHeader() // Cho phép tất cả các header
			.AllowAnyMethod() // Cho phép tất cả các phương thức HTTP (GET, POST, PUT, DELETE, v.v.)
			.AllowCredentials(); // RẤT QUAN TRỌNG: Cho phép gửi cookie, header Authorization, v.v.
								 // Nếu dùng AllowCredentials(), KHÔNG ĐƯỢC dùng AllowAnyOrigin().
		});
});


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
	app.UseSwaggerUI();
}

//app.UseExceptionHandler();
app.UseCors(MyAllowSpecificOrigins);

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<PerHue.Infrastructure.SignalR.Hub.ServerHub>("/serverhub");
app.MapControllers();

app.Run();
