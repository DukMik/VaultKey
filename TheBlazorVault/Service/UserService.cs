using Microsoft.Identity.Abstractions;
using TheApiDto;

namespace TheBlazorVault.Service
{
    /// <summary>
    /// Fournit des méthodes pour récupérer les informations de l'utilisateur connecté
    /// en appelant l'API sécurisée.
    /// </summary>
    public class UserService(IDownstreamApi downstreamApi)
    {
        private UserDto? _currentUser;

        /// <summary>
        /// Récupère les informations de l'utilisateur actuellement connecté en appelant l'API.
        /// </summary>
        /// <returns>
        /// Un objet <see cref="UserDto"/> contenant les informations de l'utilisateur ou <c>null</c>
        /// s'il n'est pas trouvé.
        /// </returns>
        public async Task<UserDto?> GetCurrentUserAsync()
        {
            // Appelle l'API sécurisée pour récupérer les infos de l'utilisateur.
            // L'endpoint "user" doit être implémenté dans votre API.
            var user = await downstreamApi.CallApiForUserAsync<UserDto>("EntraIDAuthWebAPI", options =>
                {
                    options.HttpMethod = "GET";
                    options.RelativePath = "user"; // Veillez à implémenter cet endpoint dans l'API.
                });
            return _currentUser;
        }
    }
}
