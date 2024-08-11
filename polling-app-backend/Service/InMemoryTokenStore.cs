using System.Collections.Concurrent;

namespace polling_app_backend;

public interface ITokenStore
{
    void AddToken(RefreshToken token);
    RefreshToken? GetToken(string token);
    void RemoveToken(string token);
}

public class InMemoryTokenStore : ITokenStore
{
    private readonly ConcurrentDictionary<string, RefreshToken> _tokens = new();

    public void AddToken(RefreshToken token)
    {
        _tokens[token.Token] = token;
    }

    public RefreshToken? GetToken(string token)
    {
        _tokens.TryGetValue(token, out var refreshToken);
        return refreshToken;
    }

    public void RemoveToken(string token)
    {
        _tokens.TryRemove(token, out _);
    }
}

