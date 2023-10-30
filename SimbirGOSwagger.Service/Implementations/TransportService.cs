using Microsoft.EntityFrameworkCore;
using SimbirGOSwagger.DAL.Interfaces;
using SimbirGOSwagger.Domain.Entity;
using SimbirGOSwagger.Domain.Enum;
using SimbirGOSwagger.Domain.Response;
using SimbirGOSwagger.Domain.ViewModels.Transport;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Service.Implementations;

public class TransportService : ITransportService
{
    private readonly ITransportRepository _transportRepository;

    public TransportService(ITransportRepository transportRepository)
    {
        _transportRepository = transportRepository;
    }

    public async Task<IBaseResponse<TransportViewModel>> GetTransport(int id)
    {
        try
        {
            var transport = await _transportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            if (transport == null)
            {
                return new BaseResponse<TransportViewModel>()
                {
                    Description = "Транспорт не найден",
                    StatusCode = StatusCode.TransportNotFound
                };
            }

            var transportModel = new TransportViewModel()
            {
                CanBeRented = transport.CanBeRented,
                Color = transport.Color,
                DayPrice = transport.DayPrice,
                Description = transport.Description,
                Identifier = transport.Identifier,
                Latitude = transport.Latitude,
                Longitude = transport.Longitude,
                MinutePrice = transport.MinutePrice,
                Model = transport.Model,
                TransportType = GetTransportString(transport.TransportType),
            };

            return new BaseResponse<TransportViewModel>()
            {
                Data = transportModel,
                StatusCode = StatusCode.Ok
            };
        }
        catch (Exception e)
        {
            return new BaseResponse<TransportViewModel>()
            {
                Description = "Внутрення ошибка сервера",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<string>> AddTransport(TransportViewModel model)
    {
        try
        {
            if (GetTransportType(model.TransportType) == TransportType.None)
            {
                return new BaseResponse<string>()
                {
                    Description = "Неверный тип транспорта",
                    StatusCode = StatusCode.TransportIncorrectType
                };
            }
            
            var allTransport = _transportRepository.GetAll();
            
            var newId = await allTransport.CountAsync() == 0 ? 1 : allTransport.MaxAsync(x => x.Id).Result + 1;
            
            var transport = new Transport()
            {
                Id = newId,
                CanBeRented = model.CanBeRented,
                Color = model.Color,
                DayPrice = (double)model.DayPrice!,
                Description = model.Description,
                Identifier = model.Identifier,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                MinutePrice = (double)model.MinutePrice!,
                Model = model.Model,
                TransportType = (int)GetTransportType(model.TransportType),
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

    public async Task<IBaseResponse<string>> Update(int id, TransportViewModel model)
    {
        try
        {
            if (GetTransportType(model.TransportType) == TransportType.None)
            {
                return new BaseResponse<string>()
                {
                    Description = "Неверный тип транспорта",
                    StatusCode = StatusCode.TransportIncorrectType
                };
            }
            
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
            
            transport.Description = model.Description;
            transport.TransportType = (int)GetTransportType(model.TransportType);
            transport.Color = model.Color;
            transport.Identifier = model.Identifier;
            transport.Model = model.Model;
            transport.MinutePrice = (double)model.MinutePrice!;
            transport.DayPrice = (double)model.DayPrice!;
            transport.Longitude = model.Longitude;
            transport.Latitude = model.Latitude;
            transport.CanBeRented = model.CanBeRented;
            transport.CanBeRented = model.CanBeRented;

            await _transportRepository.Update(transport);

            return new BaseResponse<string>()
            {
                Data = "Данные успешно изменены",
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
            case TransportType.All:
                return "All";
        }

        return "Error";
    }
}