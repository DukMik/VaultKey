public class LoggingContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingContextMiddleware> _logger;

    public LoggingContextMiddleware(RequestDelegate next, ILogger<LoggingContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Loguer l'en-tête Authorization
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            _logger.LogInformation("Authorization header: {AuthHeader}", authHeader.ToString());
        }
        else
        {
            _logger.LogWarning("Aucun header Authorization présent !");
        }

        // Loguer quelques claims clés
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var objectId = context.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;
            _logger.LogInformation("User authentifié avec Object Id: {ObjectId}", objectId);
        }
        else
        {
            _logger.LogInformation("L'utilisateur n'est pas authentifié.");
        }

        // Passe au middleware suivant
        await _next(context);
    }
}