using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using OpenKeyServer.Data;
using OpenKeyServer.Data.Request;
using OpenKeyServer.Data.Response;

namespace OpenKeyServer.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IConfiguration configuration, IMemoryCache memoryCache) : ControllerBase
{
    [HttpPost("get_token")]
    public IActionResult GenerateToken([FromBody] GenerateAuthToken request)
    {
        if (request.apiSecret != configuration["Secret"])
        {
            return Ok(new CommonResponse
            {
                code = (int)Code.AuthFailed
            });
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.keyId ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, request.keyId ?? "")
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? ""));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(configuration["Jwt:ExpireMinutes"] ?? "")),
            signingCredentials: creds
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);
        return Ok(new CommonResponse
        {
            code = (int)Code.Success,
            data = new TokenResponse
            {
                Token = tokenString
            }
        });
    }

    [HttpPost("get_code")]
    public IActionResult GeneratePickupCode([FromBody] GenerateCodeRequest request)
    {
        if (request.apiSecret != configuration["Secret"])
        {
            return Ok(new CommonResponse
            {
                code = (int)Code.AuthFailed
            });
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.keyId ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, request.keyId ?? "")
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? ""));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(configuration["Jwt:ExpireMinutes"] ?? "")),
            signingCredentials: creds
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);
        var options = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        var random = new Random();
        string id = random.Next(0, 1000000).ToString("D6");
        memoryCache.Set(id, tokenString, options);
        return Ok(new CommonResponse
        {
            code = (int)Code.Success,
            data = new CodeResponse
            {
                Code = id
            }
        });
    }

    [HttpPost("pickup_token")]
    public IActionResult PickupToken([FromBody] GetTokenFromCode request)
    {
        if (request.Code == null)
            return Ok(new CommonResponse
            {
                code = (int)Code.AuthFailed
            });
        memoryCache.TryGetValue(request.Code, out string? token);
        if (token == null)
            return Ok(new CommonResponse
            {
                code = (int)Code.AuthFailed
            });
        memoryCache.Remove(request.Code);
        return Ok(new CommonResponse
        {
            code = (int)Code.Success,
            data = token
        });
    }
}