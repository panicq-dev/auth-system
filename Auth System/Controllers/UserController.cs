using System.Security.Cryptography;
using System.Text;
using Auth_System.Data;
using Auth_System.Data.Dto;
using Auth_System.Models;
using Auth_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auth_System.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private AuthService _authService;
    private UserDbContext _dbContext;
    private TokenService  _tokenService;
    public UserController(AuthService authService, UserDbContext dbContext, TokenService tokenService)
    {
        _authService = authService;
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        await _authService.Register(dto);
        return Ok("User created");
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var tokens = await _authService.Login(dto);

        if (tokens is null)
        {
            return Unauthorized();
        }

        return Ok(tokens);
    }
    
    [Authorize]
    [HttpGet("protected")]
    public string ProtectedRouteTest()
    {
        return "You're authenticated!";
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
    {
        // Conversão do RefreshToken recebido nos parâmetros para um RefreshToken Hasheado.
        var hash = _tokenService.HashRefreshToken(refreshTokenDto.RefreshToken);
        // Verificação se o RefreshToken dos parâmetros hasheado corresponde a algum refreshtoken armazenado.
        var storedRefreshToken = await _dbContext.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.RefreshTokenHash == hash); // Retorna o RefreshToken Armazenado.
        
        // Se não houver RefreshToken OU se o RefreshToken estiver expirado OU se ele já foi revogado, então retorna "Sem autorização".
        var now = DateTime.UtcNow;
        if (storedRefreshToken is null ||
            !storedRefreshToken.Expires.HasValue ||
            now >= storedRefreshToken.Expires.Value ||
            storedRefreshToken.RevokeAt.HasValue)
        {
            return Unauthorized();
        }

        // Agora, se passou pelas validações anteriores, iremos INVALIDAR o RefreshToken passado e gerar um novo Access e RefreshToken.
        
        storedRefreshToken.RevokeAt = now;
        var tokenPair = _tokenService.GenerateTokenPair(storedRefreshToken.User);
        // Iremos criar um novo RefreshToken associado ao usuário & salvar no banco de dados.
        var newRefreshToken = new RefreshToken()
        {
            UserId = storedRefreshToken.UserId,
            RefreshTokenHash = tokenPair.RefreshTokenHash,
            CreatedAt = now,
            Expires = tokenPair.ExpiresAt
        };
        await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
        await _dbContext.SaveChangesAsync();
        
        AuthInfoResponse response = new AuthInfoResponse()
        {
            AcessToken = tokenPair.AcessToken,
            RefreshToken = tokenPair.RefreshToken,
        };
        return Ok(response);



    }
}