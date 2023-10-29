using Microsoft.EntityFrameworkCore;
using SimbirGOSwagger.DAL.Interfaces;
using SimbirGOSwagger.Domain.Entity;

namespace SimbirGOSwagger.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db)
    {
        _db = db;
    }
        
    public async Task Create(User entity)
    {
        await _db.User.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public IQueryable<User> GetAll()
    {
        return _db.User;
    }

    public async Task Delete(User entity)
    {
        _db.User.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<User> Update(User entity)
    {
        _db.User.Update(entity);
        await _db.SaveChangesAsync();

        return entity;
    }

    public async Task<User> GetByName(string name)
    {
        return (await _db.User.FirstOrDefaultAsync(x => x.Username == name))!;
    }
}