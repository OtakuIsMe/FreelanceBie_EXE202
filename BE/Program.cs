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
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
// using RabbitMQ.Client;
using BE.src.api.domains.eventbus;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.eventbus.Consumers;
using BE.src.api.shared.Type;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry;
using Serilog;
using Nest;
using BE.src.api.domains.Model;
using BE.src.api.domains.DTOs.ElasticSearch;
using BE.src.api.controllers;

Env.Load();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(o =>
{
	o.CustomizeProblemDetails = context =>
	{
		context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
		context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

		Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;

		if (activity?.Id != null)
		{
			context.ProblemDetails.Extensions.TryAdd("traceId", activity.Id);
		}
	};
});

// var elasticPort = ElasticSearch.Port;
// var elasticIndex = ElasticSearch.Index;

// var settings = new ConnectionSettings(new Uri(elasticPort))
// 	.DefaultIndex(elasticIndex);

// var elasticClient = new ElasticClient(settings);
// builder.Services.AddSingleton<IElasticClient>(elasticClient);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var redisConnection = BE.src.api.shared.Constant.Azure.RedisConnectionString;

var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")
	?? throw new ApplicationException("MySQL connection string not found in environment variables.");

var blogcConnectionString = BE.src.api.shared.Constant.Azure.ConnectionString;

var jwtSecretKey = BE.src.api.shared.Constant.JWT.SecretKey;

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});
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
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
				};
				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var accessToken = context.Request.Query["access_token"];
						var path = context.HttpContext.Request.Path;

						if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
						{
							context.Token = accessToken;
						}
						return Task.CompletedTask;
					}
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
								.AllowAnyHeader()
								.SetIsOriginAllowed(origin => true)
								.AllowCredentials();
					  });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();

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
builder.Services.AddScoped<IEmailServ, EmailServ>();
builder.Services.AddScoped<INotificationServ, NotificationServ>();
builder.Services.AddScoped<ISpecialtyServ, SpecialtyServ>();
builder.Services.AddScoped<ITransactionServ, TransactionServ>();
builder.Services.AddScoped<IPostServ, PostServ>();
builder.Services.AddScoped<ICommunicationServ, CommunicationServ>();
builder.Services.AddScoped<IShotServ, ShotServ>();
builder.Services.AddScoped<IAuthServ, AuthServ>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IChatServ, ChatServ>();

builder.Services.AddHostedService<RedisPollingService>();

builder.Services.AddSingleton(_ =>
{
	Console.WriteLine($"AZURE_KEY (after Env.Load()): {blogcConnectionString}");
	return new BlobServiceClient(blogcConnectionString ?? throw new ApplicationException("Azure connection string not found."));
});

Console.WriteLine($"Redis connection string: {redisConnection}");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

builder.Services.AddScoped<ICacheService, CacheServ>();

builder.Services.AddDbContext<FLBDbContext>(options =>
{
	Console.WriteLine($"Using ConnectionString: {connectionString}");
	options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

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

// builder.Services.AddSingleton<IConnectionFactory>(sp =>
// {
// 	return new ConnectionFactory()
// 	{
// 		HostName = BE.src.api.shared.Constant.Azure.RabbitMQHost,
// 		Port = int.Parse(BE.src.api.shared.Constant.Azure.RabbitMQPort),
// 		UserName = BE.src.api.shared.Constant.Azure.RabbitMQUsername,
// 		Password = BE.src.api.shared.Constant.Azure.RabbitMQPassword
// 	};
// });
// builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
// builder.Services.AddSingleton<IEventBusRabbitMQProducer, EventBusRabbitMQProducer>();
// builder.Services.AddHostedService<PostNotificationConsumer>();

builder.Services.AddScoped<IElasticSeachServ<User>, ElasticSeachServ<User>>();
builder.Services.AddScoped<IElasticSeachServ<PostJob>, ElasticSeachServ<PostJob>>();
// builder.Services.AddHostedService<ElasticsearchBackgroundService>();

Log.Logger = new LoggerConfiguration()
	.WriteTo.Seq("http://localhost:5341")
	.CreateLogger();

builder.Host.UseSerilog((context, loggerConfig) =>
{
	loggerConfig.ReadFrom.Configuration(context.Configuration);
});

builder.Logging.AddOpenTelemetry(logging =>
{
	logging.IncludeFormattedMessage = true;
	logging.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()
	.ConfigureResource(resource => resource.AddService("BE"))
	.WithTracing(tracing =>
	{
		tracing
			.AddHttpClientInstrumentation()
			.AddAspNetCoreInstrumentation()
			.AddEntityFrameworkCoreInstrumentation()
			.AddSqlClientInstrumentation(options =>
			{
				options.SetDbStatementForText = true;
			});
	})
	.UseOtlpExporter();

builder.Services.AddSignalR(e =>
{
	e.MaximumReceiveMessageSize = 102400000;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseRateLimiter();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapHub<ChatHub>("/chatHub");
});

app.UseMiddleware<AuthMiddleware>();

app.MapControllers();

app.MapGet("/", async context =>
{
	context.Response.Redirect("/swagger/index.html");
	await Task.CompletedTask;
});

app.Run();
