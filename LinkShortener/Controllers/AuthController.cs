using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LinkShortener.DAL;
using LinkShortener.Domain.Models;
using LinkShortener.Localization;
using LinkShortener.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

namespace LinkShortener.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserAuthDtoValidator _userDtoValidator;
    private readonly IStringLocalizer<ErrorMessages> _localizer;
    private readonly JwtOptions _jwtOptions;

    public AuthController(IConfiguration configuration, 
        AppDbContext db, 
        UserAuthDtoValidator userDtoValidator,
        IStringLocalizer<ErrorMessages> localizer)
    {
        _db = db;
        _userDtoValidator = userDtoValidator;
        _jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
        _localizer = localizer;
    }
    
    private string GenerateJwt(User user)
    {
        JwtSecurityToken securityToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(), ClaimValueTypes.Integer32),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            },
            expires: DateTime.Now.AddMinutes(_jwtOptions.Lifetime),
            signingCredentials: new SigningCredentials(_jwtOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256Signature)
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(securityToken);
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserAuthDto userAuth)
    {
        var validationResult = await _userDtoValidator.ValidateAsync(userAuth);
        if (!validationResult.IsValid)
            return BadRequest(new
            {
                message = validationResult.Errors.Select(e => e.ErrorMessage)
            });

        var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == userAuth.Email);

        if (dbUser == null)
            return BadRequest(new
            {
                message = _localizer["IncorrectLoginOrPassword"].Value
            });

        if (!BCrypt.Net.BCrypt.Verify(userAuth.Password, dbUser.PasswordHash))
            return BadRequest(new
            {
                message = _localizer["IncorrectLoginOrPassword"].Value
            });

        return Ok(new
        {
            id = dbUser.Id,
            jwt_token = GenerateJwt(dbUser)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserAuthDto userAuth)
    {
        var validationResult = await _userDtoValidator.ValidateAsync(userAuth);
        if (!validationResult.IsValid)
            return BadRequest(new
            {
                message = validationResult.Errors.Select(e => e.ErrorMessage)
            });

        var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == userAuth.Email);

        if (dbUser != null)
            return BadRequest(new
            {
                message = _localizer["UserAlreadyExists"].Value
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
}