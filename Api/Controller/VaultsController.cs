using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Service;
using TheApiDto;
using EntityFrameworkComm.EfModel.Context;
using EntityFrameworkComm.EfModel.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
#if !DEBUG
    [Authorize]
#endif
    public class VaultController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserService _userService;

        public VaultController(Context context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

#if DEBUG
        [AllowAnonymous]
#endif
        [HttpPost]
        public async Task<IActionResult> CreateVault([FromBody] VaultDtoCreation vaultDto)
        {
            // 1. Récupération de l'ID utilisateur
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();

            // 2. Charger l'entité User existante pour créer la relation
            var userEntity = await _context.User.FindAsync(userId);
            if (userEntity == null)
                return Unauthorized();

            // 3. Map DTO -> Entity
            var vaultEntity = new Vault
            {
                UserId = userEntity.IdUser,
                VaultName      = vaultDto.VaultName,
                DateCreated    = DateTime.UtcNow,
                KeyHash        = vaultDto.KeyHash,
                Salt           = vaultDto.Salt,
                PrivateKey     = vaultDto.PrivateKey,

                // Initialiser les collections
                Users   = new List<User>(),
                Entries = new List<Entrie>(),
                Logs    = new List<Log>()
            };

            // 4. Lier le vault à l'utilisateur via la collection many-to-many
            vaultEntity.Users.Add(userEntity);

            // 5. Enregistrer le vault
            _context.Vault.Add(vaultEntity);
            await _context.SaveChangesAsync();

            // 6. Créer et lier le log
            var logEntity = new Log
            {
                ActionDate = DateTime.UtcNow,
                ActionType = "VaultCreated",
                Details    = $"Vault '{vaultEntity.VaultName}' créé.",
                UserId     = userId,
                VaultId    = vaultEntity.IdVault,
                User       = null! // Ne pas recharger l'utilisateur, la FK suffit
            };
            vaultEntity.Logs.Add(logEntity);

            _context.Log.Add(logEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        
        /// <summary>
        /// Modifie le nom et les informations de chiffrement d'un vault existant.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVault(int id, [FromBody] VaultDtoUpdate vaultDto)
        {
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();

            var vaultEntity = await _context.Vault
                .Include(v => v.Users)
                .Include(v => v.Logs)
                .FirstOrDefaultAsync(v => v.IdVault == id);
            if (vaultEntity == null)
                return NotFound();
            if (!vaultEntity.Users.Any(u => u.IdUser == userId))
                return Unauthorized();

            // Mise à jour des champs
            vaultEntity.VaultName  = vaultDto.VaultName;
            vaultEntity.KeyHash    = vaultDto.KeyHash;
            vaultEntity.Salt       = vaultDto.Salt;
            vaultEntity.PrivateKey = vaultDto.PrivateKey;

            await _context.SaveChangesAsync();

            var logEntity = new Log
            {
                ActionDate = DateTime.UtcNow,
                ActionType = "VaultUpdated",
                Details    = $"Vault '{vaultEntity.VaultName}' modifié.",
                UserId     = userId,
                VaultId    = vaultEntity.IdVault,
                User       = null!
            };
            vaultEntity.Logs.Add(logEntity);
            _context.Log.Add(logEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("{id}/canEnter")]
        public async Task<IActionResult> CabEnterVault(int vaultId, [FromBody] Byte[] dto)
        {

            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();

            // Vérifie user ID si besoin (facultatif ici)
            var vault = await _context.Vault.FirstOrDefaultAsync(v => v.IdVault == vaultId);
            if (vault == null) return NotFound();

            // Comparer le hash envoyé à celui en base
            bool check = vault.KeyHash == dto;
            return Ok(check);
        }


        /// <summary>
        /// Active ou désactive un vault.
        /// </summary>
        /// <remarks> Pourrait etre fait autrement </remarks>
        [HttpPatch("{id}/activation")]
        public async Task<IActionResult> ToggleVaultActivation(int id, [FromBody] VaultDtoActivation activationDto)
        {
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();

            var vaultEntity = await _context.Vault
                .Include(v => v.Users)
                .Include(v => v.Logs)
                .FirstOrDefaultAsync(v => v.IdVault == id);
            if (vaultEntity == null)
                return NotFound();
            if (!vaultEntity.Users.Any(u => u.IdUser == userId))
                return Unauthorized();

            vaultEntity.IsDesactivated = activationDto.IsDesactivated;
            await _context.SaveChangesAsync();

            var logEntity = new Log
            {
                ActionDate = DateTime.UtcNow,
                ActionType = activationDto.IsDesactivated ? "VaultDeactivated" : "VaultActivated",
                Details    = $"Vault '{vaultEntity.VaultName}' {(activationDto.IsDesactivated ? "désactivé" : "activé")}.",
                UserId     = userId,
                VaultId    = vaultEntity.IdVault,
                User       = null!
            };
            vaultEntity.Logs.Add(logEntity);
            _context.Log.Add(logEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }
    
    }
}
