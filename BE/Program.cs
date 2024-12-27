using BE.src.api.domains.Database;
using BE.src.api.repositories;
using BE.src.api.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FLBDbContext>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMembershipRepo, MembershipRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddScoped<IMembershipServ, MembershipServ>();
builder.Services.AddScoped<IUserServ, UserServ>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
