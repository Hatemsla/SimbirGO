using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.Transport;

namespace SimbirGOSwagger.Service.Interfaces;

public interface IAdminTransportService
{
    Task<IBaseResponse<IEnumerable<Transport>>> GetTransports(int start, int count, string transportType);
    Task<IBaseResponse<Transport>> GetTransport(int id);
    Task<IBaseResponse<string>> AddTransport(AdminTransportViewModel model);
    Task<IBaseResponse<string>> Update(int id, AdminTransportViewModel model);
    Task<IBaseResponse<string>> Delete(int id);
}