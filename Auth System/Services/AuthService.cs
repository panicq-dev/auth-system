using Auth_System.Data;
using Auth_System.Data.Dto;
using Auth_System.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth_System.Services;

public class AuthService
{
    private TokenService  _tokenService;
    private IMapper _mapper;
    private SignInManager<User> _signInManager;
    private UserDbContext _dbContext;
    public AuthService(TokenService tokenService, IMapper mapper, SignInManager<User> signInManager, UserDbContext dbContext)
    {
        _tokenService = tokenService;
        _mapper = mapper;
        _signInManager = signInManager;
        _dbContext = dbContext;
    }

    public async Task<string> Register(UserRegisterDto dto)
    {

        var user = _mapper.Map<User>(dto);
        var createUser = await _signInManager.UserManager.CreateAsync(user, dto.Password);
        if (!createUser.Succeeded)
        {
            return "[ERROR] User not created";
        }
        return "[SUCCESS] User created";
    }


    public async Task<AuthInfoResponse> Login(UserLoginDto dto)
    {
        // Verificando se o usuário existe, de acordo com o dto passado. Retorna o único usuário, ou null.
       var user =  await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == dto.UserName);
       if (user is null) 
       {
           return null;
       }

       var passwordVerification = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
       if (!passwordVerification.Succeeded)
       {
           return null;
       }
       // Geração de RefreshToken e Token + criação de um objeto RefreshToken.
       var tokens = _tokenService.GenerateTokenPair(user);
        _dbContext.RefreshTokens.Add(new RefreshToken()
        {
            UserId = user.Id,
            RefreshTokenHash = tokens.RefreshTokenHash,
            Expires = tokens.ExpiresAt,
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();
        
        var response = new AuthInfoResponse()
        {
            RefreshToken = tokens.RefreshToken,
            AcessToken = tokens.AcessToken,
        };
        return response;
    }
}