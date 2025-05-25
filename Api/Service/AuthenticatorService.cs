using System.Collections.Concurrent;

namespace Api.Service;

public class AuthenticatorService 
{
    // Durée de validité d'une connexion (ex: 30 minutes)
    private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(60);

    // Structure pour stocker la connexion
    private record UserVaultConnection(int UserId, int VaultId, DateTime ConnectedAt);

    // Liste thread-safe des connexions actives
    private readonly ConcurrentBag<UserVaultConnection> _connections = new();

    /// <summary>
    /// Enregistre la connexion d'un utilisateur à un vault.
    /// </summary>
    public void RegisterConnection(int userId, int vaultId)
    {
        _connections.Add(new UserVaultConnection(userId, vaultId, DateTime.UtcNow));
    }

    /// <summary>
    /// Vérifie si la connexion est toujours valide.
    /// </summary>
    public bool IsConnectionValid(int userId, int vaultId)
    {
        var now = DateTime.UtcNow;
        return _connections.Any(c =>
            c.UserId == userId &&
            c.VaultId == vaultId &&
            (now - c.ConnectedAt) <= ConnectionTimeout
        );
    }

    
}