using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace TheBlazorVault.Components.AtomCore
{
    /// <summary>
    /// Fournit des méthodes d'extension pour configurer les routes d'authentification,
    /// incluant les endpoints de connexion et de déconnexion.
    /// </summary>
    /// <remarks>
    /// Ce composant est dédié à la configuration des endpoints relatifs à l'authentification en utilisant
    /// à la fois l'authentification par cookies et le protocole OpenIdConnect.
    /// La méthode <see cref="MapLoginAndLogout(IEndpointRouteBuilder)"/> configure notamment les points d'accès
    /// pour la connexion (/login) et la déconnexion (/logout).
    /// </remarks>
    internal static class LoginLogoutEndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Configure et mappe les endpoints pour la connexion et la déconnexion sur le routeur d'endpoints fourni.
        /// </summary>
        /// <param name="endpoints">
        /// L'instance de <see cref="IEndpointRouteBuilder"/> utilisée pour définir les endpoints de l'application.
        /// </param>
        /// <returns>
        /// Un objet <see cref="IEndpointConventionBuilder"/> permettant de chaîner des conventions supplémentaires.
        /// </returns>
        internal static IEndpointConventionBuilder MapLoginAndLogout(this IEndpointRouteBuilder endpoints)
        {
            // Crée un groupe d'endpoints sans préfixe de route spécifique.
            var group = endpoints.MapGroup(string.Empty);

            // Configure l'endpoint de connexion.
            // Redirige l'utilisateur vers une action de défi (challenge) d'authentification avec les propriétés définies.
            group.MapGet("/login", (string? returnUrl) => TypedResults.Challenge(GetAuthProperties(returnUrl)))
                .AllowAnonymous();

            // Configure l'endpoint de déconnexion.
            // Effectue la déconnexion des schémas d'authentification Cookie et OpenIdConnect,
            // afin d'éviter une reconnexion automatique de l'utilisateur si un schéma n'est pas correctement déconnecté.
            group.MapPost("/logout", ([FromForm] string? returnUrl) => TypedResults.SignOut(
                GetAuthProperties(returnUrl),
                new[] { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme }
            ));

            return group;
        }

        /// <summary>
        /// Crée une instance de <see cref="AuthenticationProperties"/> pour configurer le processus d'authentification,
        /// notamment afin de prévenir les attaques de redirection ouverte.
        /// </summary>
        /// <param name="returnUrl">
        /// L'URL de retour fournie. Si elle est non nulle, une URI absolue est attendue et seule la partie chemin et query est utilisée.
        /// Si elle est nulle, la redirection par défaut sera vers la racine ("/").
        /// </param>
        /// <returns>
        /// Une instance de <see cref="AuthenticationProperties"/> avec la propriété <see cref="AuthenticationProperties.RedirectUri"/>
        /// configurée en fonction de <paramref name="returnUrl"/>.
        /// </returns>
        private static AuthenticationProperties GetAuthProperties(string? returnUrl) =>
            new()
            {
                // Utilise le pattern switch pour extraire le chemin et la query d'une URI absolue ou retourner "/" par défaut.
                RedirectUri = returnUrl switch
                {
                    string => new Uri(returnUrl, UriKind.Absolute).PathAndQuery,
                    null => "/",
                }
            };
    }
}
