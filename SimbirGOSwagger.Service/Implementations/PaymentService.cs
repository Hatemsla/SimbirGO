using Microsoft.EntityFrameworkCore;
using SimbirGOSwagger.DAL.Interfaces;
using SimbirGOSwagger.Domain.Enum;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Service.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IUserRepository _userRepository;

    public PaymentService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<IBaseResponse<string>> AddBalance(int id, string username)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            var currentUser = await _userRepository.GetByName(username);
            
            if (user == null || currentUser == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            if (!currentUser.IsAdmin)
            {
                if (currentUser != user)
                {
                    return new BaseResponse<string>()
                    {
                        Description = "Отказано в доступе",
                        StatusCode = StatusCode.AccessDenied
                    };
                }
            }

            user.Balance += 250000;
            await _userRepository.Update(user);
            
            return new BaseResponse<string>()
            {
                Description = "Перевед осуществлен",
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