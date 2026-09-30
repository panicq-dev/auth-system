using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth_System.Data.Dto;
using Auth_System.Models;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;

namespace Auth_System.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    public TokenService(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _mapper = mapper;
    }

    public string CreateToken(UserLoginDto dto)
    {
        var user = _mapper.Map<User>(dto);

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
}