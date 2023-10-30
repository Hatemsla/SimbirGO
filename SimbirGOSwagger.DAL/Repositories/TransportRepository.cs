using SimbirGOSwagger.DAL.Interfaces;
using SimbirGOSwagger.Domain.Entity;

namespace SimbirGOSwagger.DAL.Repositories;

public class TransportRepository : ITransportRepository
{
    private readonly ApplicationDbContext _db;

    public TransportRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task Create(Transport entity)
    {
        await _db.Transport.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public IQueryable<Transport> GetAll()
    {
        return _db.Transport;
    }

    public async Task Delete(Transport entity)
    {
        _db.Transport.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<Transport> Update(Transport entity)
    {
        _db.Transport.Update(entity);
        await _db.SaveChangesAsync();

        return entity;
    }
}