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
    private readonly IValidator<UserAuthDto> _registerValidator;

    public AuthController(IConfiguration configuration,
        AppDbContext db,
        IValidator<UserAuthDto> registerValidator)
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
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(), ClaimValueTypes.Integer32),
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
    public async Task<IActionResult> Register([FromBody] UserAuthDto userAuth)
    {
        var validationResult = await _registerValidator.ValidateAsync(userAuth);
        if (!validationResult.IsValid)
            return BadRequest(new
            {
                message = validationResult.Errors.Select(e => e.ErrorMessage)
            });

        var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == userAuth.Email);

        if (dbUser != null)
            return BadRequest(new
            {
                message = "Пользователь с такой почтой уже существует"
            });

        var newUser = new User()
        {
            Email = userAuth.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userAuth.Password)
        };

        await _db.Users.AddAsync(newUser);
        await _db.SaveChangesAsync();

        return Created("", new
        {
            id = newUser.Id,
            jwt_token = GenerateJwt(newUser)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserAuthDto userAuth)
    {
        var validationResult = await _registerValidator.ValidateAsync(userAuth);
        if (!validationResult.IsValid)
            return BadRequest(new
            {
                message = validationResult.Errors.Select(e => e.ErrorMessage)
            });

        var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == userAuth.Email);

        if (dbUser == null)
            return BadRequest(new
            {
                message = "Неверный логин или пароль"
            });

        if (!BCrypt.Net.BCrypt.Verify(userAuth.Password, dbUser.PasswordHash))
            return BadRequest(new
            {
                message = "Неверный логин или пароль"
            });

        return Ok(new
        {
            id = dbUser.Id,
            jwt_token = GenerateJwt(dbUser)
        });
    }
}