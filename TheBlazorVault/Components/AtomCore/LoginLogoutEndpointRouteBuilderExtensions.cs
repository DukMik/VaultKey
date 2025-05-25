using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace TheBlazorVault.Components.AtomCore
{
    
    internal static class LoginLogoutEndpointRouteBuilderExtensions
    {
        
        internal static IEndpointConventionBuilder MapLoginAndLogout(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup(string.Empty);
            
            group.MapGet("/login", (string? returnUrl) => TypedResults.Challenge(GetAuthProperties(returnUrl)))
                .AllowAnonymous();

       //     // Configure l'endpoint de déconnexion.
       //     // Effectue la déconnexion des schémas d'authentification Cookie et OpenIdConnect,
       //     // afin d'éviter une reconnexion automatique de l'utilisateur si un schéma n'est pas correctement déconnecté.
       //     group.MapPost("/logout", ([FromForm] string? returnUrl) => TypedResults.SignOut(GetAuthProperties(returnUrl),
       //         new[] { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme }
       //     ));

           // return group;
            
            // Sign out with both the Cookie and OIDC authentication schemes. Users who have not signed out with the OIDC scheme will
            // automatically get signed back in as the same user the next time they visit a page that requires authentication
            // with no opportunity to choose another account.
            group.MapPost("/logout", ([FromForm] string? returnUrl) => TypedResults.SignOut(GetAuthProperties(returnUrl),
                [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));

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
                    not null => new Uri(returnUrl, UriKind.Absolute).PathAndQuery,
                    null => "/",
                }
            };
    }
}



