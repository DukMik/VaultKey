using Api.Repositories;

namespace Api.Service
{
    /// <summary>
    /// Service de gestion de l'utilisateur applicatif, qui délègue les opérations de récupération/création au repository.
    /// </summary
    public class UserService(UserRepositorie userRepositorie)
    {       

        /// <summary>
        /// Identifiant interne courant de l'utilisateur. Sert souvent à conserver le contexte utilisateur dans la chaîne d'exécution.
        /// </summary>
        public int CurrentUserId { get; set; }

        public async Task<int> GetOrCreateAppUserIdAsync(Guid externalUserId) => await userRepositorie.GetOrCreateAppUserIdAsync(externalUserId);
    }
}
