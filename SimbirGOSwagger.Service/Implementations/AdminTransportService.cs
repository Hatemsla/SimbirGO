using Microsoft.EntityFrameworkCore;
using SimbirGOSwagger.DAL.Interfaces;
using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Enum;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.Transport;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Service.Implementations;

public class AdminTransportService : IAdminTransportService
{
    private readonly ITransportRepository _transportRepository;
    private readonly IUserRepository _userRepository;

    public AdminTransportService(ITransportRepository transportRepository, IUserRepository userRepository)
    {
        _transportRepository = transportRepository;
        _userRepository = userRepository;
    }

    public async Task<IBaseResponse<IEnumerable<Transport>>> GetTransports(int start, int count, string transportType)
    {
        try
        {
            var transports = await _transportRepository.GetAll()
                .Where(transport => transport.Id >= start && transport.TransportType == (int)GetTransportType(transportType))
                .OrderBy(transport => transport.Id)
                .Take(count)
                .ToListAsync();

            if (transports == null || !transports.Any())
            {
                return new BaseResponse<IEnumerable<Transport>>()
                {
                    Description = "Транспорт не найден",
                    StatusCode = StatusCode.TransportNotFound
                };
            }

            return new BaseResponse<IEnumerable<Transport>>()
            {
                Data = transports,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<IEnumerable<Transport>>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<Transport>> GetTransport(int id)
    {
        try
        {
            var transport = await _transportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            if (transport == null)
            {
                return new BaseResponse<Transport>()
                {
                    Description = "Транспорт не найден",
                    StatusCode = StatusCode.TransportNotFound
                };
            }

            return new BaseResponse<Transport>()
            {
                Data = transport,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<Transport>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> AddTransport(AdminTransportViewModel model)
    {
        try
        {
            var allTransports = _transportRepository.GetAll();
            
            var newId = await allTransports.CountAsync() == 0 ? 1 : allTransports.MaxAsync(x => x.Id).Result + 1;

            if (GetTransportType(model.TransportType) == TransportType.All ||
                GetTransportType(model.TransportType) == TransportType.None)
            {
                return new BaseResponse<string>()
                {
                    Description = "Неверный тип транспорта",
                    StatusCode = StatusCode.TransportIncorrectType
                };
            }

            var users = _userRepository.GetAll();

            if (!users.Any(x => x.Id == model.OwnerId))
            {
                return new BaseResponse<string>()
                {
                    Description = "Владелец транспорта не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }
            
            var transport = new Transport()
            {
                Id = newId,
                Owner = model.OwnerId,
                CanBeRented = model.CanBeRented,
                TransportType = (int)GetTransportType(model.TransportType),
                Model = model.Model,
                Color = model.Color,
                Identifier = model.Identifier,
                Description = model.Description,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                MinutePrice = (double)model.MinutePrice!,
                DayPrice = (double)model.DayPrice!
            };

            await _transportRepository.Create(transport);

            return new BaseResponse<string>()
            {
                Data = "Транспорт успешно добавлен",
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<string>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> Update(int id, AdminTransportViewModel model)
    {
        try
        {
            var allTransports = _transportRepository.GetAll();

            var transport = await allTransports.FirstOrDefaultAsync(x => x.Id == id);

            if (transport == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Транспорт не найден",
                    StatusCode = StatusCode.TransportNotFound
                };
            }
            
            var users = _userRepository.GetAll();

            if (!users.Any(x => x.Id == model.OwnerId))
            {
                return new BaseResponse<string>()
                {
                    Description = "Владелец транспорта не найден",
                    StatusCode = StatusCode.UserNotFound
                };
            }
            
            if (GetTransportType(model.TransportType) == TransportType.All ||
                GetTransportType(model.TransportType) == TransportType.None)
            {
                return new BaseResponse<string>()
                {
                    Description = "Неверный тип транспорта",
                    StatusCode = StatusCode.TransportIncorrectType
                };
            }

            transport.Owner = model.OwnerId;
            transport.CanBeRented = model.CanBeRented;
            transport.TransportType = (int)GetTransportType(model.TransportType);
            transport.Model = model.Model;
            transport.Color = model.Color;
            transport.Identifier = model.Identifier;
            transport.Description = model.Description;
            transport.Latitude = model.Latitude;
            transport.Longitude = model.Longitude;
            transport.MinutePrice = (double)model.MinutePrice!;
            transport.DayPrice = (double)model.DayPrice!;
            
            await _transportRepository.Update(transport);

            return new BaseResponse<string>()
            {
                Data = "Транспорт успешно изменен",
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<string>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> Delete(int id)
    {
        try
        {
            var allTransport = _transportRepository.GetAll();

            var transport = await allTransport.FirstOrDefaultAsync(x => x.Id == id);
            
            if (transport == null)
            {
                return new BaseResponse<string>()
                {
                    Description = "Транспорт не найден",
                    StatusCode = StatusCode.TransportNotFound
                };
            }
            
            await _transportRepository.Delete(transport);

            return new BaseResponse<string>()
            {
                Data = "Транспорт успешно удален",
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<string>()
            {
                Description = "Внутренняя ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
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