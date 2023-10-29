using SimbirGOSwagger.Domain.Enum;

namespace SimbirGOSwagger.Domain.Response;

public class BaseResponse<T> : IBaseResponse<T>
{
    public T Data { get; set; }
    public string Description { get; set; }
    public StatusCode StatusCode { get; set; }
}

public interface IBaseResponse<T>
{
    T Data { get; set; }
    string Description { get; set; }
    StatusCode StatusCode { get; set; }
}