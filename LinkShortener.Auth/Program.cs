using System.Diagnostics;
using FluentValidation;
using LinkShortener.Auth.Common;
using LinkShortener.Auth.Models;
using LinkShortener.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

services.AddScoped<AppDbContext>();
services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();

services.AddApiVersioning();
services.AddControllers();
services.AddRouting();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidAudience = jwtOptions.Audience,
            ValidIssuer = jwtOptions.Issuer,
            IssuerSigningKey = jwtOptions.GetSymmetricSecurityKey(),
            
        };
    });
services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UsePathBase(new PathString("/api/"));
app.UseApiVersioning();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(e => { e.MapControllers(); });

app.Run();