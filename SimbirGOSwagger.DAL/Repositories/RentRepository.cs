using SimbirGOSwagger.DAL.Interfaces;
using SimbirGOSwagger.Domain.Entity;

namespace SimbirGOSwagger.DAL.Repositories;

public class RentRepository : IRentRepository
{
    private readonly ApplicationDbContext _db;

    public RentRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Create(Rent entity)
    {
        await _db.Rent.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public IQueryable<Rent> GetAll()
    {
        return _db.Rent;
    }

    public async Task Delete(Rent entity)
    {
        _db.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<Rent> Update(Rent entity)
    {
        _db.Rent.Update(entity);
        await _db.SaveChangesAsync();

        return entity;
    }
}