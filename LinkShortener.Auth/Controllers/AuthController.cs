using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using LinkShortener.Auth.Common;
using LinkShortener.Auth.Models;
using LinkShortener.DAL;
using LinkShortener.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LinkShortener.Auth.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/v{version:apiVersion}/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly JwtOptions _jwt;
    private readonly AppDbContext _db;
    private readonly IValidator<RegisterDto> _registerValidator;

    public AuthController(IConfiguration configuration, 
        AppDbContext db,
        IValidator<RegisterDto> registerValidator)
    {
        _jwt = configuration.GetSection("JwtOptions").Get<JwtOptions>();
        _registerValidator = registerValidator;
        _db = db;
    }

    private string GenerateJwt(User user)
    {
        JwtSecurityToken securityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(),ClaimValueTypes.Integer32),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            },
            expires: DateTime.Now.AddMinutes(_jwt.Lifetime),
            signingCredentials: new SigningCredentials(_jwt.GetSymmetricSecurityKey(), 
                SecurityAlgorithms.HmacSha256Signature)
            );

        var tokenHandler = new JwtSecurityTokenHandler();
        
        return tokenHandler.WriteToken(securityToken);
    }
    
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerUser)
    {
        var validationResult = await _registerValidator.ValidateAsync(registerUser);
        if (!validationResult.IsValid)
            return BadRequest(new
            {
                message = validationResult.Errors.Select(e => e.ErrorMessage)
            });

        var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == registerUser.Email);

        if (dbUser != null)
            return BadRequest(new
            {
                message = "Пользователь с такой почтой уже существует"
            });

        var newUser = await _db.Users.AddAsync(new User()
        {
            Email = registerUser.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUser.Password)
        });
        
        await _db.SaveChangesAsync();

        return Created("", new
        {
            id = newUser.Entity.Id,
            jwt_token = GenerateJwt(newUser.Entity)
        });
    }
    
    // public JsonResult Login()
    // {
    //     
    // }
    //

}