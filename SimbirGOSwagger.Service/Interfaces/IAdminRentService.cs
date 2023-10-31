using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.Rent;

namespace SimbirGOSwagger.Service.Interfaces;

public interface IAdminRentService
{
    Task<IBaseResponse<Rent>> GetRentHistory(int id);
    Task<IBaseResponse<IEnumerable<RentTransportHistoryViewModel>>> GetUserHistory(int userId);
    Task<IBaseResponse<IEnumerable<RentUserHistoryViewModel>>> GetTransportHistory(int transportId);
    Task<IBaseResponse<string>> NewRent(AdminNewRentViewModel model);
    Task<IBaseResponse<string>> EndRent(int rentId, double latitude, double longitude);
    Task<IBaseResponse<string>> EditRent(int rendId, AdminNewRentViewModel model);
    Task<IBaseResponse<string>> DeleteRent(int renId);
}