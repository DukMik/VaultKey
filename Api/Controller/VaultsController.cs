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
    [Authorize]
#if DEBUG
    [AllowAnonymous]
#endif
    public class VaultController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserService _userService;
        private readonly AuthenticatorService _authenticatorService;

        public VaultController(Context context, UserService userService, AuthenticatorService authenticatorService)
        {
            _context = context;
            _userService = userService;
            _authenticatorService = authenticatorService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateVault([FromBody] VaultDtoCreation vaultDto)
        {
            // 1. Récupération de l'ID utilisateur => ne pas faire confiance qu client 
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();

            // 2. Charger l'entité User existante pour créer la relation
            var userEntity = await _context.User.FindAsync(userId);
            if (userEntity == null)
                return Unauthorized();
            
            var vaultEntity = new Vault
            {
                UserId = userEntity.IdUser,
                VaultName      = vaultDto.VaultName,
                DateCreated    = DateTime.UtcNow,
                KeyHash        = vaultDto.KeyHash,
                Salt           = vaultDto.Salt,
                PrivateKey     = vaultDto.PrivateKey,
                Users   = new List<User>(),
                Entries = new List<Entrie>(),
                Logs    = new List<Log>()
            };
           
            vaultEntity.Users.Add(userEntity);
          
            _context.Vault.Add(vaultEntity);
            await _context.SaveChangesAsync();
           
            var logEntity = new Log
            {
                ActionDate = DateTime.UtcNow,
                ActionType = "VaultCreated",
                Details    = $"Vault '{vaultEntity.VaultName}' créé.",
                UserId     = userId,
                VaultId    = vaultEntity.IdVault,
                User       = null!
            };
            vaultEntity.Logs.Add(logEntity);

            _context.Log.Add(logEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        
        /// <summary>
        /// Modifie le nom et les informations de chiffrement d'un vault existant.
        /// </summary>
        [HttpPut("{vaultId}")]
        public async Task<IActionResult> UpdateVault(int vaultId, [FromBody] VaultDtoUpdate vaultDto)
        {
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();

            var vaultEntity = await _context.Vault
                .Include(v => v.Users)
                .Include(v => v.Logs)
                .FirstOrDefaultAsync(v => v.IdVault == vaultId);
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

        /// <summary>
        /// Active ou désactive un vault.
        /// </summary>
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
        
        /// <summary>
        /// permet de savoir si l'utilisateur peut se connecter ou non aux entries 
        /// </summary>
        /// <param name="vaultId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("{vaultId}/canEnter")]
        public async Task<IActionResult> CanEnterVault(int vaultId, [FromBody] byte[] dto)
        {
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();
            
            var vault = await _context.Vault.FirstOrDefaultAsync(v => v.IdVault == vaultId);
            if (vault == null) return NotFound();

            bool check = vault.KeyHash.SequenceEqual(dto);

            if (check)
            {
                _authenticatorService.RegisterConnection(userId, vaultId);
            }

            return Ok(check);
        }
    }
}
