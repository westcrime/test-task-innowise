using FluentValidation;
using FluentValidation.AspNetCore;
using Inno_Shop.Users.API.Extensions;
using Inno_Shop.Users.API.Infrastructure;
using Inno_Shop.Users.API.Middlewares;
using Inno_Shop.Users.API.Validators;
using Inno_Shop.Users.Application.Repositories;
using Inno_Shop.Users.Application.Services;
using Inno_Shop.Users.Application.Services.Email;
using Inno_Shop.Users.Application.Services.Hash;
using Inno_Shop.Users.Application.Services.Token;
using Inno_Shop.Users.Application.Validators;
using Inno_Shop.Users.Infrastructure.JwtOptions;
using Inno_Shop.Users.Infrastructure.Services.Email;
using Inno_Shop.Users.Infrastructure.Services.Hash;
using Microsoft.AspNetCore.Identity;
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

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<LoginUserDtoValidator>();

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<AddUserDtoValidator>();

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<ForgotPasswordDtoValidator>();

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<UpdateUserDtoValidator>();

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<UpdateUserForAdminDtoValidator>();

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<LoginUserDtoValidator>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=InnoShopDB.sqlite"));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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

app.UseExceptionHandler();

app.MapControllers();

app.Run();
