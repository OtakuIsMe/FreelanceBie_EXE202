using System.Security.Claims;
using System.Text;
using BE.src.api.domains.Database;
using BE.src.api.domains.Enum;
using BE.src.api.repositories;
using BE.src.api.services;
using BE.src.api.shared.Constant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using DotNetEnv;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;

Env.Load();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION")
	?? throw new InvalidOperationException("Connection string not found in environment variables.");

builder.Services.AddControllers();
builder.Services.AddControllersWithViews()
	.AddNewtonsoftJson(options =>
	options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = JWT.Issuer,
					ValidAudience = JWT.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT.SecretKey))
				};
			});

builder.Services.AddAuthorization(options =>
					{
						options.AddPolicy("Customer", policy =>
							policy.RequireClaim(ClaimTypes.Role, RoleEnum.Customer.ToString()));
						options.AddPolicy("Admin", policy =>
							policy.RequireClaim(ClaimTypes.Role, RoleEnum.Admin.ToString()));
						options.AddPolicy("Staff", policy =>
							policy.RequireClaim(ClaimTypes.Role, RoleEnum.Staff.ToString()));
					});

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: MyAllowSpecificOrigins,
					  policy =>
					  {
						  policy.WithOrigins("http://localhost:5173")
								.AllowAnyMethod()
								.AllowAnyHeader();
					  });
});

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IMembershipRepo, MembershipRepo>();
builder.Services.AddScoped<INotificationRepo, NotificationRepo>();
builder.Services.AddScoped<ISocialProfileRepo, SocialProfileRepo>();
builder.Services.AddScoped<ISpecialtyRepo, SpecialtyRepo>();
builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddScoped<ICommunicationRepo, CommunicationRepo>();
builder.Services.AddScoped<IShotRepo, ShotRepo>();

builder.Services.AddScoped<IUserServ, UserServ>();
builder.Services.AddScoped<IMembershipServ, MembershipServ>();
builder.Services.AddSingleton<EmailServ>();
builder.Services.AddScoped<INotificationServ, NotificationServ>();
builder.Services.AddScoped<ISpecialtyServ, SpecialtyServ>();
builder.Services.AddScoped<ITransactionServ, TransactionServ>();
builder.Services.AddScoped<IPostServ, PostServ>();
builder.Services.AddScoped<ICommunicationServ, CommunicationServ>();
builder.Services.AddScoped<IShotServ, ShotServ>();

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddScoped<ICacheService, CacheServ>();

builder.Services.AddDbContext<FLBDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "Enter 'Bearer' [space] and your token. Example: Bearer abc123xyz"
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddRateLimiter(_ => _
	.AddFixedWindowLimiter(policyName: "fixed", options =>
	{
		options.PermitLimit = 4;
		options.Window = TimeSpan.FromSeconds(12);
		options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		options.QueueLimit = 2;
	}));

builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(5000);
	options.ListenAnyIP(5147);
	options.ListenAnyIP(5148);
	options.ListenAnyIP(5149);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello from ASP.NET Core!");

app.Run();
