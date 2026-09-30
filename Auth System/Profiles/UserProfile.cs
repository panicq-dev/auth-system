using Auth_System.Data.Dto;
using Auth_System.Models;
using AutoMapper;

namespace Auth_System.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserLoginDto, User>();
        CreateMap<UserRegisterDto, User>();
    }
}