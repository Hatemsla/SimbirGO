using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.User;

namespace SimbirGOSwagger.Service.Interfaces;

public interface IAdminUserService
{
    Task<IBaseResponse<User>> GetUser(int id);
    Task<IBaseResponse<IEnumerable<User>>> GetUsers(int start, int count);
    Task<IBaseResponse<string>> Create(AdminUserViewModel model);
    Task<IBaseResponse<string>> Update(int id, AdminUserViewModel model);
    Task<IBaseResponse<string>> Delete(int id);
}