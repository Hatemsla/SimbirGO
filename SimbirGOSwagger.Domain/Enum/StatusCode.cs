namespace SimbirGOSwagger.Domain.Enum;

public enum StatusCode
{
    Ok,
    UserNotFound,
    InternalServerError,
    UserAlreadyExists,
    AccessDenied,
    TransportNotFound,
    TransportIncorrectType,
    RentIncorrectType,
    RentNotFound
}