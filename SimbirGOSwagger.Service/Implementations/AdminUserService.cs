using Microsoft.EntityFrameworkCore;
using SimbirGOSwagger.DAL.Interfaces;
using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Enum;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.User;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Service.Implementations;

public class AdminUserService : IAdminUserService
{
    private readonly IUserRepository _userRepository;

    public AdminUserService(IUserRepository userRepository)
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

    public async Task<IBaseResponse<IEnumerable<User>>> GetUsers(int start, int count)
    {
        try
        {
            var users = await _userRepository.GetAll()
                .Where(user => user.Id >= start)
                .OrderBy(user => user.Id)
                .Take(count)
                .ToListAsync();

            if (users == null || !users.Any())
            {
                return new BaseResponse<IEnumerable<User>>()
                {
                    Description = "Пользователи не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            return new BaseResponse<IEnumerable<User>>()
            {
                Data = users,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<IEnumerable<User>>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> Create(AdminUserViewModel model)
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
                Password = model.Password,
                IsAdmin = model.IsAdmin,
                Balance = model.Balance
            };

            await _userRepository.Create(user);

            return new BaseResponse<string>()
            {
                Data = "Аккаунт успешно создан",
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

    public async Task<IBaseResponse<string>> Update(int id, AdminUserViewModel model)
    {
        try
        {
            var allUsers = _userRepository.GetAll();

            var user = await allUsers.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }
            
            user.Username = model.Username;
            user.Password = model.Password;
            user.IsAdmin = model.IsAdmin;
            user.Balance = model.Balance;

            await _userRepository.Update(user);

            return new BaseResponse<string>()
            {
                Data = "Данные успешно изменены",
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

    public async Task<IBaseResponse<string>> Delete(int id)
    {
        try
        {
            var allUsers = _userRepository.GetAll();

            var user = await allUsers.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }
            
            await _userRepository.Delete(user);

            return new BaseResponse<string>()
            {
                Data = "Аккаунт успешно удален",
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