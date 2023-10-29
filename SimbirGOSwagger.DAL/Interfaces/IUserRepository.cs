using SimbirGOSwagger.Domain.Entity;

namespace SimbirGOSwagger.DAL.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User> GetByName(string name);
}