using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.User;

namespace SimbirGOSwagger.Service.Interfaces;

public interface IUserService
{
    Task<IBaseResponse<UserDetailViewModel>> GetUserByName(string name);
    Task<IBaseResponse<string>> SignUp(UserViewModel model);
    Task<IBaseResponse<string>> SignIn(UserViewModel model);
    Task<IBaseResponse<UserViewModel>> Update(UserViewModel model, string currentName);
}