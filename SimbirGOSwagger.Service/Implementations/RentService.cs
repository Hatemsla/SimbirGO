using Microsoft.EntityFrameworkCore;
using SimbirGOSwagger.DAL.Interfaces;
using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Enum;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.Rent;
using SimbirGOSwagger.Domain.ViewModels.Transport;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Service.Implementations;

public class RentService : IRentService
{
    private readonly IRentRepository _rentRepository;
    private readonly ITransportRepository _transportRepository;
    private readonly IUserRepository _userRepository;

    public RentService(IRentRepository rentRepository, ITransportRepository transportRepository, IUserRepository userRepository)
    {
        _rentRepository = rentRepository;
        _transportRepository = transportRepository;
        _userRepository = userRepository;
    }

    public async Task<IBaseResponse<IEnumerable<RentTransportViewModel>>> GetRentTransport(double latitude, double longitude, double radius, string type)
    {
        try
        {
            var transports = _transportRepository.GetAll();

            var transportType = GetTransportType(type);

            if (transportType == TransportType.None)
            {
                return new BaseResponse<IEnumerable<RentTransportViewModel>>()
                {
                    Description = "Неверный тип транспорта",
                    StatusCode = StatusCode.TransportIncorrectType
                };
            }

            var filteredTransportsType = await transports.ToListAsync();
            
            if (transportType != TransportType.All)
            {
                filteredTransportsType = filteredTransportsType
                    .Where(t => t.TransportType == (int)GetTransportType(type)).ToList();
            }

            var degreesLat = radius / 111.0; // 1 градус широты = 111 км (приближенно)
            var degreesLon = radius / (111.0 * Math.Cos(Math.PI * latitude / 180.0)); // Поправка на долготу

            var minLat = latitude - degreesLat;
            var maxLat = latitude + degreesLat;
            var minLon = longitude - degreesLon;
            var maxLon = longitude + degreesLon;
            
            var filteredTransportsByDistance = filteredTransportsType
                .Where(t => t.Latitude >= minLat && t.Latitude <= maxLat)
                .Where(t => t.Longitude >= minLon && t.Longitude <= maxLon)
                .ToList();

            var rents = await _rentRepository.GetAll().Select(r => r.TransportId).ToListAsync();
            
            var availableTransports = filteredTransportsByDistance
                .Where(t => !rents.Contains(t.Id));

            if (!availableTransports.Any())
            {
                return new BaseResponse<IEnumerable<RentTransportViewModel>>()
                {
                    Description = "Не обнаружено доступных транспортных средств для аренды",
                    StatusCode = StatusCode.TransportNotFound
                };
            }
            
            var transportViewModels = availableTransports.Select(t => new RentTransportViewModel()
            {
                TransportType = GetTransportString(t.TransportType),
                Model = t.Model,
                Color = t.Color,
                Identifier = t.Identifier,
                Description = t.Description,
                Latitude = t.Latitude,
                Longitude = t.Longitude,
                MinutePrice = t.MinutePrice,
                DayPrice = t.DayPrice
            });

            return new BaseResponse<IEnumerable<RentTransportViewModel>>()
            {
                Data = transportViewModels,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<IEnumerable<RentTransportViewModel>>()
            {
                Description = "Внутрення ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> NewRent(string username, int transportId, string rentType)
    {
        try
        {
            if (rentType is not ("Minuets" or "Days"))
                return new BaseResponse<string>()
                {
                    Description = "Неверный тип аренды",
                    StatusCode = StatusCode.RentIncorrectType
                };
            
            var user = await _userRepository.GetByName(username);

            if (user == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            var transport = await _transportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == transportId);

            if (transport == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Транспорт не найден",
                    StatusCode = StatusCode.TransportNotFound
                };
            }

            if (transport.Owner == user.Id)
            {
                return new BaseResponse<string>()
                {
                    Description = "Нельзя брать в аренду собственный транспорт",
                    StatusCode = StatusCode.AccessDenied
                };
            }

            var currentRent = await _rentRepository.GetAll().FirstOrDefaultAsync(x =>
                x.TransportId == transport.Id && x.EndDate == null);

            if (currentRent != null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Транспорт уже арендован",
                    StatusCode = StatusCode.AccessDenied
                };
            }

            var newId = await _rentRepository.GetAll().CountAsync() == 0 ? 1 : _rentRepository.GetAll().MaxAsync(x => x.Id).Result + 1;
            
            var rent = new Rent()
            {
                Id = newId,
                UserId = user.Id,
                TransportId = transport.Id,
                StartDate = DateTime.UtcNow,
                RentId = rentType is "Days" ? 2 : 1
            };

            await _rentRepository.Create(rent);

            return new BaseResponse<string>()
            {
                Data = "Транспорт успешно арендован",
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

    public async Task<IBaseResponse<string>> EndRent(int rentId, string username, double latitude, double longitude)
    {
        try
        {
            var user = await _userRepository.GetByName(username);

            if (user == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            var rent = await _rentRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == rentId && x.UserId == user.Id && x.EndDate == null);

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

    public async Task<IBaseResponse<IEnumerable<RentTransportHistoryViewModel>>> GetHistory(string username)
    {
        try
        {
            var user = await _userRepository.GetByName(username);

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

    public async Task<IBaseResponse<RentTransportHistoryViewModel>> GetRentInfo(string username, int id)
    {
        try
        {
            var user = await _userRepository.GetByName(username);

            if (user == null)
            {
                return new BaseResponse<RentTransportHistoryViewModel>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            var rent = await _rentRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            if (rent == null)
            {
                return new BaseResponse<RentTransportHistoryViewModel>()
                {
                    Description = "Аренда не найдена",
                    StatusCode = StatusCode.RentNotFound
                };
            }

            var owner = await _transportRepository.GetAll().FirstOrDefaultAsync(x => x.Owner == user.Id);
            var tenant = rent.UserId == user.Id;

            if (owner == null)
            {
                return new BaseResponse<RentTransportHistoryViewModel>()
                {
                    Description = "Отказано в доступе",
                    StatusCode = StatusCode.AccessDenied
                };
            }
            
            if (tenant)
            {
                return new BaseResponse<RentTransportHistoryViewModel>()
                {
                    Description = "Отказано в доступе",
                    StatusCode = StatusCode.AccessDenied
                };
            }
            
            var transport = await _transportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == rent.TransportId);

            if (transport == null)
            {
                return new BaseResponse<RentTransportHistoryViewModel>()
                {
                    Description = "Транспорт не найден",
                    StatusCode = StatusCode.TransportNotFound
                };
            }

            var rentView = new RentTransportHistoryViewModel()
            {
                TransportType = GetTransportString(transport.TransportType),
                Model = transport.Model,
                Color = transport.Color,
                Identifier = transport.Identifier,
                RentType = rent.RentId == 1 ? "Minuets" : "Days",
                StartDate = rent.StartDate,
                EndDate = rent.EndDate
            };

            return new BaseResponse<RentTransportHistoryViewModel>()
            {
                Data = rentView,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<RentTransportHistoryViewModel>()
            {
                Description = "Внутрення ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<RentUserHistoryViewModel>>> GetTransportHistory(int transportId,
        string username)
    {
        try
        {
            var owner = await _userRepository.GetByName(username);

            if (owner == null)
            {
                return new BaseResponse<IEnumerable<RentUserHistoryViewModel>>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }

            var transport = await _transportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == transportId && x.Owner == owner.Id);

            if (transport == null)
            {
                return new BaseResponse<IEnumerable<RentUserHistoryViewModel>>()
                {
                    Description = "Доступ только для владельца транспорта",
                    StatusCode = StatusCode.AccessDenied
                };
            }
            
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