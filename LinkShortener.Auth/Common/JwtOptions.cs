using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LinkShortener.Auth.Common;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Key { get; set; }
    public int Lifetime { get; set; } // in minutes
    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}