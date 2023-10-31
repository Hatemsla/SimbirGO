using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.Rent;
using SimbirGOSwagger.Domain.ViewModels.Transport;

namespace SimbirGOSwagger.Service.Interfaces;

public interface IRentService
{
    Task<IBaseResponse<IEnumerable<RentTransportViewModel>>> GetRentTransport(double latitude, double longitude, double radius, string type);
    Task<IBaseResponse<string>> NewRent(string username, int transportId, string rentType);
    Task<IBaseResponse<string>> EndRent(int rentId, string username, double latitude, double longitude);
    Task<IBaseResponse<IEnumerable<RentTransportHistoryViewModel>>> GetHistory(string username);
    Task<IBaseResponse<RentTransportHistoryViewModel>> GetRentInfo(string username, int id);
    Task<IBaseResponse<IEnumerable<RentUserHistoryViewModel>>> GetTransportHistory(int transportId,
        string username);
}