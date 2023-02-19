using LinkShortener.Auth.Common;
using LinkShortener.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Stravaig.ShortCode;
using Stravaig.ShortCode.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

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

services.AddApiVersioning();
services.AddControllers();

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
            IssuerSigningKey = jwtOptions.GetSymmetricSecurityKey()
        };
    });
services.AddAuthentication();

var app = builder.Build();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(e => { e.MapControllers(); });
app.MapGet("/", () => "Hello World!");

app.Run();