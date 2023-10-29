using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Service.Implementations;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly HashSet<string> _invalidTokens = new HashSet<string>();
    
    public void InvalidateToken(string token)
    {
        _invalidTokens.Add(token);
    }

    public bool IsTokenInvalid(string token)
    {
        return _invalidTokens.Contains(token);
    }
}