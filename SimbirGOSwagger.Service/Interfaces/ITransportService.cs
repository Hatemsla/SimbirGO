using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.Transport;

namespace SimbirGOSwagger.Service.Interfaces;

public interface ITransportService
{
    Task<IBaseResponse<TransportViewModel>> GetTransport(int id);
    Task<IBaseResponse<string>> AddTransport(TransportViewModel model);
    Task<IBaseResponse<string>> Update(int id, TransportViewModel model);
    Task<IBaseResponse<string>> Delete(int id);
}