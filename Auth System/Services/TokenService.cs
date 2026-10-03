using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth_System.Data;
using Auth_System.Data.Dto;
using Auth_System.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth_System.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(User user)
    {
        
        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var token = new JwtSecurityToken(
            issuer: _configuration["JWTTokenConfiguration:Issuer"],
            audience: _configuration["JWTTokenConfiguration:Audience"],
            expires: DateTime.Now.AddMinutes(30),
            claims: claims,
            signingCredentials: creds
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        var bytes = new byte[128];
        using var randomNumber = RandomNumberGenerator.Create();
        randomNumber.GetBytes(bytes);
        var refreshToken = Convert.ToBase64String(bytes);
        return refreshToken;
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
    public DateTime GetRefreshTokenExpiration()
    {
        var minutes = _configuration.GetValue<int>("JWTTokenConfiguration:RefreshExpireInMinutes");
        if (minutes <= 0)
        {
            throw new InvalidOperationException("JWTTokenConfiguration:RefreshExpireInMinutes deve ser maior que zero.");
        }
        return DateTime.UtcNow.AddMinutes(minutes);
    }
    public AuthInfo GenerateTokenPair(User user)
    {
        // Gerando AcessToken e RefreshToken.
        var token = CreateToken(user);
        var refreshToken = CreateRefreshToken();
        var refreshTokenHash = HashRefreshToken(refreshToken);
        var refreshTokenExpirationTime = GetRefreshTokenExpiration();
        // Retornando todas as informações para o AuthService Login.
        return new AuthInfo()
        {
            AcessToken= token,
            RefreshToken = refreshToken,
            RefreshTokenHash =  refreshTokenHash,
            ExpiresAt = refreshTokenExpirationTime
        };
    }

    
}

