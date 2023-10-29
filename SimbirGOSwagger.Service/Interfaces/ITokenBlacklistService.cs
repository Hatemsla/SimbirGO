namespace SimbirGOSwagger.Service.Interfaces;

public interface ITokenBlacklistService
{
    void InvalidateToken(string token);
    bool IsTokenInvalid(string token);
}