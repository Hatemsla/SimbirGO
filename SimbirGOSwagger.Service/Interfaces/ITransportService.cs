using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.Transport;

namespace SimbirGOSwagger.Service.Interfaces;

public interface ITransportService
{
    Task<IBaseResponse<TransportViewModel>> GetTransport(int id);
    Task<IBaseResponse<string>> AddTransport(string username, TransportViewModel model);
    Task<IBaseResponse<string>> Update(string username, int transportId, TransportViewModel model);
    Task<IBaseResponse<string>> Delete(string username, int transportId);
}