using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[Authorize]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    [HttpGet("context")]
    public IActionResult GetContext()
    {
        // Extraire et renvoyer les claims de l'utilisateur authentifié
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}