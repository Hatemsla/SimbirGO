using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimbirGOSwagger.DAL.Interfaces;
using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Enum;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.User;
using SimbirGOSwagger.Service.Helpers;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Service.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<IBaseResponse<User>> GetUser(int id)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return new BaseResponse<User>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            return new BaseResponse<User>()
            {
                Data = user,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<User>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
    
    public async Task<IBaseResponse<UserDetailViewModel>> GetUserByName(string name)
    {
        try
        {
            var user = await _userRepository.GetByName(name);

            if (user == null)
            {
                return new BaseResponse<UserDetailViewModel>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            var model = new UserDetailViewModel()
            {
                Username = user.Username,
                Password = user.Password,
                Balance = user.Balance
            };

            return new BaseResponse<UserDetailViewModel>()
            {
                Data = model,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<UserDetailViewModel>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
    
    public async Task<IBaseResponse<string>> SignIn(UserViewModel model)
    {
        try
        {
            var allUsers = _userRepository.GetAll();

            var user = await allUsers.FirstOrDefaultAsync(x => x.Username == model.Username);

            if (user == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }
            
            var claims = new List<Claim> { new(ClaimTypes.Name, user.Username) };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new BaseResponse<string>()
            {
                Data = new JwtSecurityTokenHandler().WriteToken(jwt),
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<string>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<UserViewModel>> Update(UserViewModel model, string currentName)
    {
        try
        {
            var allUsers = _userRepository.GetAll();

            var user = await allUsers.FirstOrDefaultAsync(x => x.Username == currentName);

            if (user == null)
            {
                return new BaseResponse<UserViewModel>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }
            
            user.Username = model.Username;
            user.Password = model.Password;

            await _userRepository.Update(user);

            return new BaseResponse<UserViewModel>()
            {
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<UserViewModel>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> SignUp(UserViewModel model)
    {
        try
        {
            var allUsers = _userRepository.GetAll();

            var user = await allUsers.FirstOrDefaultAsync(x => x.Username == model.Username);

            if (user != null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Пользователь с таким именем уже существует",
                    StatusCode = StatusCode.UserAlreadyExists
                };
            }
            
            var newId = await allUsers.CountAsync() == 0 ? 1 : allUsers.MaxAsync(x => x.Id).Result + 1;

            user = new User()
            {
                Id = newId,
                Username = model.Username,
                Password = model.Password
            };

            await _userRepository.Create(user);
            
            var claims = new List<Claim> { new(ClaimTypes.Name, user.Username) };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new BaseResponse<string>()
            {
                Data = new JwtSecurityTokenHandler().WriteToken(jwt),
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<string>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
}