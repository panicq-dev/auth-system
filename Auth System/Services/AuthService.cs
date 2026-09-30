using Auth_System.Data.Dto;
using Auth_System.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Auth_System.Services;

public class AuthService
{
    private TokenService  _tokenService;
    private IMapper _mapper;
    private SignInManager<User> _signInManager;

    public AuthService(TokenService tokenService, IMapper mapper, SignInManager<User> signInManager)
    {
        _tokenService = tokenService;
        _mapper = mapper;
        _signInManager = signInManager;
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

    public string Login(UserLoginDto dto)
    {
        var token = _tokenService.CreateToken(dto);
        return token;
    }
}