using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheApiDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Service;
using EntityFrameworkComm.EfModel.Context;
using EntityFrameworkComm.EfModel.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/vault/{vaultId}/entries")]
#if !DEBUG
    [Authorize]
#endif
    public class EntrieController : ControllerBase
    {
        private readonly Context _dbContext;
        private readonly UserService _userService;

        public EntrieController(Context dbContext, UserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        #if DEBUG
        [AllowAnonymous]
#endif
        [HttpPost]
        public async Task<IActionResult> CreateEntrie(int vaultId,[FromBody] EntrieDtoCreation entriedto)
        {
            // 1. Vérifier l'utilisateur connecté
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();

            // 2. Vérifier que le vault appartient à l'utilisateur
            var vault = await _dbContext.Vault
                .Include(v => v.Users)
                .FirstOrDefaultAsync(v => v.IdVault == vaultId);
            if (vault == null || !vault.Users.Any(u => u.IdUser == userId))
                return Unauthorized();

            // 3. Créer l'entité Entrie (EF Core)
            var entry = new Entrie
            {
                VaultId        = vaultId,
                CreatedDate    = DateTime.UtcNow,
                UpdatedDate   = DateTime.UtcNow,
                IsDesactivated = entriedto.IsDesactivated,
                Vault          = null!, // Ne pas recharger le vault, la FK suffit
                Logs           = null!, // Ne pas recharger les logs, la FK suffit
                EncryptedData  = null!
            };
            _dbContext.Set<Entrie>().Add(entry);
            await _dbContext.SaveChangesAsync();

            // 4. Créer les EncryptedData (EF Core)
            var encryptedEntries = new List<EncryptedData>
            {
                new EncryptedData { EntrieId = entry.IdEntrie, Iv = entriedto.NameData.Iv, CryptedData = entriedto.NameData.CryptedData, Tag = entriedto.NameData.Tag, Entrie = null!, Logs = null!},
                new EncryptedData { EntrieId = entry.IdEntrie, Iv = entriedto.UserNameData.Iv, CryptedData = entriedto.UserNameData.CryptedData, Tag = entriedto.UserNameData.Tag, Entrie = null!, Logs = null! },
                new EncryptedData { EntrieId = entry.IdEntrie, Iv = entriedto.UrlData.Iv, CryptedData = entriedto.UrlData.CryptedData, Tag = entriedto.UrlData.Tag, Entrie = null!, Logs = null! },
                new EncryptedData { EntrieId = entry.IdEntrie, Iv = entriedto.CommentData.Iv, CryptedData = entriedto.CommentData.CryptedData, Tag = entriedto.CommentData.Tag, Entrie = null!, Logs = null! },
                new EncryptedData { EntrieId = entry.IdEntrie, Iv = entriedto.PasswordData.Iv, CryptedData = entriedto.PasswordData.CryptedData, Tag = entriedto.PasswordData.Tag, Entrie = null!, Logs = null! }
            };
            _dbContext.Set<EncryptedData>().AddRange(encryptedEntries);
            await _dbContext.SaveChangesAsync();

            // 5. Mettre à jour les clés étrangères dans Entrie
            entry.NameDataId     = encryptedEntries[0].IdEncryptedData;
            entry.UserNameDataId = encryptedEntries[1].IdEncryptedData;
            entry.UrlDataId      = encryptedEntries[2].IdEncryptedData;
            entry.CommentDataId  = encryptedEntries[3].IdEncryptedData;
            entry.PasswordDataId = encryptedEntries[4].IdEncryptedData;
            await _dbContext.SaveChangesAsync();

            // 6. Créer un log de création d'entrée
            var logEntry = new Log
            {
                ActionDate = DateTime.UtcNow,
                ActionType = "EntryCreated",
                Details    = $"Entrée {entry.IdEntrie} créée dans le vault {vaultId}.",
                UserId     = userId,
                VaultId    = vaultId,
                EntryId    = entry.IdEntrie,
                
                User = null! // Ne pas recharger l'utilisateur, la FK suffit'
            };
            _dbContext.Log.Add(logEntry);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    
        
#if DEBUG
        [AllowAnonymous]
#endif
        [HttpGet]
        public async Task<IActionResult> GetEntries(int vaultId)
        {
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();

            // Vérifier que le vault appartient à l'utilisateur
            var vault = await _dbContext.Set<Vault>()
                .Include(v => v.Users)
                .FirstOrDefaultAsync(v => v.IdVault == vaultId);
            if (vault == null)
                return NotFound();
            if (!vault.Users.Any(u => u.IdUser == userId))
                return Unauthorized();

            // Récupérer les entrées
            var entries = await _dbContext.Set<Entrie>()
                .Where(e => e.VaultId == vaultId)
                .ToListAsync();

            // Mapper pour la réponse
            var result = new List<EntrieListDto>();
            foreach (var e in entries)
            {
                // Charger les données chiffrées
                var nameData     = await _dbContext.Set<EncryptedData>().FindAsync(e.NameDataId);
                var userNameData = await _dbContext.Set<EncryptedData>().FindAsync(e.UserNameDataId);
                var urlData      = await _dbContext.Set<EncryptedData>().FindAsync(e.UrlDataId);
                var commentData  = await _dbContext.Set<EncryptedData>().FindAsync(e.CommentDataId);

                result.Add(new EntrieListDto
                {
                    IdEntrie       = e.IdEntrie,
                    CreatedDate    = e.CreatedDate,
                    UpdatedDate   = e.UpdatedDate,
                    IsDesactivated = e.IsDesactivated,
                    NameData       = nameData != null ? Convert.ToBase64String(nameData.CryptedData) : string.Empty,
                    UserNameData   = userNameData != null ? Convert.ToBase64String(userNameData.CryptedData) : string.Empty,
                    UrlData        = urlData != null ? Convert.ToBase64String(urlData.CryptedData) : string.Empty,
                    CommentData    = commentData != null ? Convert.ToBase64String(commentData.CryptedData) : string.Empty,
                    PasswordData   = "******"
                });
            }

            // 4. Journaliser l'affichage des entrées
            var logListEntity = new Log
            {
                ActionDate = DateTime.UtcNow,
                ActionType = "EntriesListed",
                Details    = $"Affichage des entrées du vault {vaultId}.",
                UserId     = userId,
                VaultId    = vaultId,
                EntryId    = null,
                User       = null!
            };
            _dbContext.Set<Log>().Add(logListEntity);
            await _dbContext.SaveChangesAsync();

            return Ok(result);
        }

#if DEBUG
        [AllowAnonymous]
#endif
        [HttpGet("{entryId}/password")]
        public async Task<IActionResult> GetEntryPassword(int vaultId, int entryId)
        {
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();

            // Vérifier l'existence et l'appartenance
            var vault = await _dbContext.Set<Vault>()
                .Include(v => v.Users)
                .FirstOrDefaultAsync(v => v.IdVault == vaultId);
            if (vault == null || !vault.Users.Any(u => u.IdUser == userId))
                return Unauthorized();

            var entry = await _dbContext.Set<Entrie>()
                .FirstOrDefaultAsync(e => e.IdEntrie == entryId && e.VaultId == vaultId);
            if (entry == null)
                return NotFound();

            var pwdData = await _dbContext.Set<EncryptedData>()
                .FirstOrDefaultAsync(d => d.IdEncryptedData == entry.NameDataId /* remplacer par PasswordDataId une fois ajouté */);
            if (pwdData == null)
                return NotFound();

            // 4. Journaliser l'affichage du mot de passe
            var logPwdEntity = new Log
            {
                ActionDate = DateTime.UtcNow,
                ActionType = "EntryPasswordViewed",
                Details    = $"Affichage du mot de passe pour l'entrée {entryId} du vault {vaultId}.",
                UserId     = userId,
                VaultId    = vaultId,
                EntryId    = entryId,
                User       = null!
            };
            _dbContext.Set<Log>().Add(logPwdEntity);
            await _dbContext.SaveChangesAsync();

            var dto = new EntryPasswordDto
            {
                IdEntrie     = entry.IdEntrie,
                PasswordData = Convert.ToBase64String(pwdData.CryptedData),
                Iv           = Convert.ToBase64String(pwdData.Iv),
                Tag          = Convert.ToBase64String(pwdData.Tag)
            };
            return Ok(dto);
        }
    }
}
