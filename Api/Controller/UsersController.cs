using Microsoft.AspNetCore.Mvc;
using TheApiDto;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkComm.EfModel.Context;
using Api.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;




namespace Api.Controller
{
    /// <summary>
    /// Contrôleur permettant la gestion des utilisateurs et l'extraction de leurs données associées.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]// S'assure que seul un utilisateur connecté peut accéder
    public class UsersController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserService _userService;
        private readonly AuthenticatorService _authenticatorService;

        public UsersController(Context context, UserService userService, AuthenticatorService authenticatorService)
        {
            _context = context;
            _userService = userService;
            _authenticatorService = authenticatorService;
        }

        [HttpGet("user")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {  
            
            var user = await _context.User
                .Where(u => u.IdUser == _userService.CurrentUserId)
                .Select(u => new UserDto
                {
                    IdUser = u.IdUser,
                    EntraIdUser = u.EntraIdUser,
                    Vaults = null,
                    Logs = null
                })
                .FirstOrDefaultAsync();
            
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            return Ok(user);
        }
        
        /// <summary>
        /// Récupère l'identifiant interne (IdUser) de l'utilisateur actuellement connecté.
        /// </summary>
        [HttpGet("id")]
        public async Task<ActionResult<int>> GetCurrentUserId()
        {
            // Récupère l'identifiant externe (OID ou sub) depuis les claims
            var externalIdStr = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
                                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                                ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(externalIdStr) || !Guid.TryParse(externalIdStr, out var externalUserId))
            {
                return Unauthorized("Impossible de déterminer l'identité externe de l'utilisateur.");
            }

            // Utilise le service pour obtenir ou créer l'ID interne
            var appUserId = await _userService.GetOrCreateAppUserIdAsync(externalUserId);

            return Ok(appUserId);
        }     

        [HttpGet("IsConnectionValid/{vaultId}")]
        public async Task<ActionResult<bool>> IsConnectionValid(int vaultId)
        {
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();
            
            bool isValid = await Task.Run(() => _authenticatorService.IsConnectionValid(userId, vaultId));
            return Ok(new { Value = isValid });
        }
        
        /// <summary>
        /// Récupère la liste des coffres (vaults) d’un utilisateur spécifié par son identifiant.
        /// </summary>
        [HttpGet("vaults")]
        public async Task<ActionResult<IEnumerable<VaultDto>>> GetVaultsForCurrentUser()
        {
            var userId = _userService.CurrentUserId;

            if (userId == int.MinValue)
                return Unauthorized();
            
            var vaults = await _context.Vault
                .Where(v => v.UserId == userId && !v.IsDesactivated)
                .Select(v => new VaultDto
                {
                    IdVault = v.IdVault,
                    UserId = v.UserId,
                    VaultName = v.VaultName,
                    DateCreated = v.DateCreated,
                    KeyHash = v.KeyHash,
                    Salt = v.Salt,
                    PrivateKey = v.PrivateKey,
                    IsDesactivated = v.IsDesactivated
                })
                .ToListAsync();

            return Ok(vaults);
        }
        
        /// <summary>
        /// Récupère le coffre (vault) d’un utilisateur spécifié par son identifiant.
        /// </summary>
        [HttpGet("vault/{id}")]
        public async Task<ActionResult<VaultDto>> GetOneVaultForCurrentUser(int id)
        {
            var userId = _userService.CurrentUserId;

            if (userId == int.MinValue)
                return Unauthorized();
            
            var vault = await _context.Vault
                .Where(v => v.IdVault == id && !v.IsDesactivated)
                .Select(v => new VaultDto
                {
                    IdVault = v.IdVault,
                    UserId = v.UserId,
                    VaultName = v.VaultName,
                    DateCreated = v.DateCreated,
                    KeyHash = v.KeyHash,
                    Salt = v.Salt,
                    PrivateKey = v.PrivateKey,
                    IsDesactivated = v.IsDesactivated
                })
                .FirstOrDefaultAsync();

            return Ok(vault);
        }
    }
}
