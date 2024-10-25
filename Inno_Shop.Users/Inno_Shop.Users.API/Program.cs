using Inno_Shop.Users.API.Extensions;
using Inno_Shop.Users.API.Middlewares;
using Inno_Shop.Users.Application.Repositories;
using Inno_Shop.Users.Application.Services;
using Inno_Shop.Users.Application.Services.Email;
using Inno_Shop.Users.Application.Services.Hash;
using Inno_Shop.Users.Application.Services.Token;
using Inno_Shop.Users.Infrastructure.JwtOptions;
using Inno_Shop.Users.Infrastructure.Services.Email;
using Inno_Shop.Users.Infrastructure.Services.Hash;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddApiAuthentication(builder.Services.BuildServiceProvider().GetService<IOptions<JwtOptions>>());

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashService, HashService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=InnoShopDB.sqlite"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseWhen(context => !(context.Request.Path.StartsWithSegments("/api/Users/login") || context.Request.Path.StartsWithSegments("/api/Users/register")
|| context.Request.Path.StartsWithSegments("/api/Users/verify") || context.Request.Path.StartsWithSegments("/api/Users/send-code")), appBuilder =>
{
    appBuilder.UseMiddleware<VerificationMiddleware>();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
