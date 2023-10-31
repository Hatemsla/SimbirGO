using Microsoft.EntityFrameworkCore;
using SimbirGOSwagger.DAL.Interfaces;
using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Enum;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.Rent;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Service.Implementations;

public class AdminRentService : IAdminRentService
{
    private readonly IRentRepository _rentRepository;
    private readonly ITransportRepository _transportRepository;
    private readonly IUserRepository _userRepository;

    public AdminRentService(IRentRepository rentRepository, ITransportRepository transportRepository, IUserRepository userRepository)
    {
        _rentRepository = rentRepository;
        _transportRepository = transportRepository;
        _userRepository = userRepository;
    }

    public async Task<IBaseResponse<Rent>> GetRentHistory(int id)
    {
        try
        {
            var rent = await _rentRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            if (rent == null)
            {
                return new BaseResponse<Rent>()
                {
                    Description = "Аренда не найдена",
                    StatusCode = StatusCode.RentNotFound
                };
            }

            return new BaseResponse<Rent>()
            {
                Data = rent,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<Rent>()
            {
                Description = "Внутрення ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<RentTransportHistoryViewModel>>> GetUserHistory(int userId)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return new BaseResponse<IEnumerable<RentTransportHistoryViewModel>>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            var rents = await _rentRepository.GetAll()
                .Where(x => x.UserId == user.Id && x.EndDate != null)
                .ToListAsync();

            var transportIds = rents.Select(r => r.TransportId).ToList();
            
            var transports = await _transportRepository.GetAll()
                .Where(t => transportIds.Contains(t.Id))
                .ToListAsync();
            
            var rentHistoryViewModels = rents.Select(r => new RentTransportHistoryViewModel
            {
                TransportType = GetTransportString(transports.FirstOrDefault(t => t.Id == r.TransportId)!.TransportType),
                Model = transports.FirstOrDefault(t => t.Id == r.TransportId)!.Model,
                Color = transports.FirstOrDefault(t => t.Id == r.TransportId)!.Color,
                Identifier = transports.FirstOrDefault(t => t.Id == r.TransportId)!.Identifier,
                RentType = r.RentId == 1 ? "Minuets" : "Days",
                StartDate = r.StartDate,
                EndDate = r.EndDate
            }).ToList();

            return new BaseResponse<IEnumerable<RentTransportHistoryViewModel>>()
            {
                Data = rentHistoryViewModels,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<IEnumerable<RentTransportHistoryViewModel>>()
            {
                Description = "Внутрення ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<RentUserHistoryViewModel>>> GetTransportHistory(int transportId)
    {
        try
        {
            var rents = await _rentRepository.GetAll().Where(x => x.TransportId == transportId && x.EndDate != null)
                .ToListAsync();

            var userIds = rents.Select(r => r.UserId).ToList();

            var users = await _userRepository.GetAll()
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
            
            var rentUserHistoryViewModels = rents.Select(r => new RentUserHistoryViewModel
            {
                Username = users.FirstOrDefault(u => u.Id == r.UserId)!.Username,
                RentType = r.RentId == 1 ? "Minuets" : "Days",
                StartDate = r.StartDate,
                EndDate = r.EndDate
            }).ToList();

            return new BaseResponse<IEnumerable<RentUserHistoryViewModel>>()
            {
                Data = rentUserHistoryViewModels,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<IEnumerable<RentUserHistoryViewModel>>()
            {
                Description = "Внутрення ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> NewRent(AdminNewRentViewModel model)
    {
        try
        {
            var rent = await _rentRepository.GetAll()
                .FirstOrDefaultAsync(x => x.UserId == model.UserId && x.TransportId == model.TransportId && x.EndDate != null);

            if (rent != null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Транспорт уже арендован",
                    StatusCode = StatusCode.AccessDenied
                };
            }
            
            var newId = await _rentRepository.GetAll().CountAsync() == 0 ? 1 : _rentRepository.GetAll().MaxAsync(x => x.Id).Result + 1;

            if (model.PriceType is not ("Days" or "Minuets"))
                return new BaseResponse<string>()
                {
                    Description = "Неверный тип аренды",
                    StatusCode = StatusCode.RentIncorrectType
                };
            
            rent = new Rent()
            {
                Id = newId,
                UserId = model.UserId,
                TransportId = model.TransportId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                RentId = model.PriceType == "Days" ? 2 : 1,
                PriceOfUnit = model.PriceOfUnit,
                FinalPrice = (double)model.FinalPrice!
            };

            await _rentRepository.Create(rent);

            return new BaseResponse<string>()
            {
                Data = "Аренда успешно создана",
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<string>()
            {
                Description = "Внутрення ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> EndRent(int rentId, double latitude, double longitude)
    {
        try
        {
            var rent = await _rentRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == rentId && x.EndDate == null);

            if (rent == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Аренда не найдена",
                    StatusCode = StatusCode.RentNotFound
                };
            }
            
            rent.EndDate = DateTime.UtcNow;

            var transport = await _transportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == rent.TransportId);

            if (transport == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Транспорт не найден",
                    StatusCode = StatusCode.TransportNotFound
                };
            }

            transport.Longitude = longitude;
            transport.Latitude = latitude;

            await _rentRepository.Update(rent);
            await _transportRepository.Update(transport);

            return new BaseResponse<string>()
            {
                Data = "Аренда успешно завершена",
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<string>()
            {
                Description = "Внутрення ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> EditRent(int rendId, AdminNewRentViewModel model)
    {
        try
        {
            var rent = await _rentRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == rendId);

            if (rent == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Аренда не найдена",
                    StatusCode = StatusCode.RentNotFound
                };
            }
            
            if (model.PriceType is not ("Days" or "Minuets"))
                return new BaseResponse<string>()
                {
                    Description = "Неверный тип аренды",
                    StatusCode = StatusCode.RentIncorrectType
                };
            
            rent.UserId = model.UserId;
            rent.TransportId = model.TransportId;
            rent.StartDate = model.StartDate;
            rent.EndDate = model.EndDate;
            rent.RentId = model.PriceType == "Days" ? 2 : 1;
            rent.PriceOfUnit = model.PriceOfUnit;
            rent.FinalPrice = (double)model.FinalPrice!;

            await _rentRepository.Update(rent);

            return new BaseResponse<string>()
            {
                Data = "Аренда успешно обновлена",
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<string>()
            {
                Description = "Внутрення ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> DeleteRent(int renId)
    {
        try
        {
            var rent = await _rentRepository.GetAll().FirstOrDefaultAsync(x => x.Id == renId);

            if (rent == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Аренда не найдена",
                    StatusCode = StatusCode.RentNotFound
                };
            }

            await _rentRepository.Delete(rent);
            
            return new BaseResponse<string>()
            {
                Data = "Аренда успешно удалена",
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<string>()
            {
                Description = "Внутрення ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    private string GetTransportString(int transportType)
    {
        switch ((TransportType)transportType)
        {
            case TransportType.Car:
                return "Car";
            case TransportType.Bike:
                return "Bike";
            case TransportType.Scooter:
                return "Scooter";
        }

        return "Error";
    }
    
    private TransportType GetTransportType(string transportString)
    {
        switch (transportString)
        {
            case "Car":
                return TransportType.Car;
            case "Bike":
                return TransportType.Bike;
            case "Scooter":
                return TransportType.Scooter;
            case "All":
                return TransportType.All;
        }

        return TransportType.None;
    }
}