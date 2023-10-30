using SimbirGOSwagger.Domain.Response;

namespace SimbirGOSwagger.Service.Interfaces;

public interface IPaymentService
{
    Task<IBaseResponse<string>> AddBalance(int id, string username);
}