using Api.Service;
using Microsoft.Identity.Web;

namespace Api.Middleware
{
    /// <summary>
    /// Middleware qui obtient ou crée l'identifiant interne de l'utilisateur à partir de son identifiant externe.
    /// </summary>
    public class GetOrCreateAppUserIdMiddleware(UserService userService) : IMiddleware
    {     
        /// <summary>
        /// Exécute la logique du middleware : vérifie l'authentification, récupère l'ID externe, et obtient ou crée l'utilisateur.
        /// </summary>
        /// <param name="context">Contexte HTTP de la requête.</param>
        /// <param name="next">Délégué pour appeler le prochain middleware dans la chaîne.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                // Vérifie si l'utilisateur est authentifié et tente de récupérer son identifiant externe
                if ((context.User.Identity?.IsAuthenticated ?? false) == false
                    || !Guid.TryParse(context.User.GetObjectId(), out Guid externalUserId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                else
                {
                    // Utilise le service pour récupérer ou créer l'utilisateur et stocke l'identifiant interne courant
                    userService.CurrentUserId = await userService.GetOrCreateAppUserIdAsync(externalUserId);

                    // Passe la main au middleware suivant de la chaîne
                    await next(context);
                }
            }
            catch (Exception)
            {
                // En cas d'exception, la réponse retourne une erreur 500.
                // Optionnel : une journalisation de l'erreur peut être ajoutée ici.
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}
