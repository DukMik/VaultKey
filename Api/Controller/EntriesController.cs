﻿using TheApiDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Service;
using EntityFrameworkComm.EfModel.Context;
using EntityFrameworkComm.EfModel.Models;

namespace Api.Controller
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
        private readonly AuthenticatorService _authenticatorService;

        public EntrieController(Context context, UserService userService, AuthenticatorService authenticatorService)
        {
            _dbContext = context;
            _userService = userService;
            _authenticatorService = authenticatorService;
        }

#if DEBUG
        [AllowAnonymous]
#endif
        [HttpPost]
        public async Task<IActionResult> CreateEntrie(int vaultId, [FromBody] EntrieDtoCreation entrieDtoCreation)
        {
            var entriedto = entrieDtoCreation;
            
            var userId = _userService.CurrentUserId;
            if (userId == 0)
                return Unauthorized();
            
            var vault = await _dbContext.Vault
                .Include(v => v.Users)
                .FirstOrDefaultAsync(v => v.IdVault == vaultId);
            if (vault == null || !vault.Users.Any(u => u.IdUser == userId))
                return Unauthorized();
            
            if (!_authenticatorService.IsConnectionValid(userId, vaultId))
                return Unauthorized();
            
            var entry = new Entrie
            {
                VaultId = vaultId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                IsDesactivated = entriedto.IsDesactivated,
                Vault = null!,
                Logs = null!,
                EncryptedData = null!
            };
            _dbContext.Set<Entrie>().Add(entry);
            await _dbContext.SaveChangesAsync();
            
            var encryptedEntries = new List<EncryptedData>
            {
                new EncryptedData
                {
                    EntrieId = entry.IdEntrie, Iv = entriedto.NameData.Iv, CryptedData = entriedto.NameData.CryptedData,
                    Tag = entriedto.NameData.Tag, Entrie = null!, Logs = null!
                },
                new EncryptedData
                {
                    EntrieId = entry.IdEntrie, Iv = entriedto.UserNameData.Iv,
                    CryptedData = entriedto.UserNameData.CryptedData, Tag = entriedto.UserNameData.Tag, Entrie = null!,
                    Logs = null!
                },
                new EncryptedData
                {
                    EntrieId = entry.IdEntrie, Iv = entriedto.UrlData.Iv, CryptedData = entriedto.UrlData.CryptedData,
                    Tag = entriedto.UrlData.Tag, Entrie = null!, Logs = null!
                },
                new EncryptedData
                {
                    EntrieId = entry.IdEntrie, Iv = entriedto.CommentData.Iv,
                    CryptedData = entriedto.CommentData.CryptedData, Tag = entriedto.CommentData.Tag, Entrie = null!,
                    Logs = null!
                },
                new EncryptedData
                {
                    EntrieId = entry.IdEntrie, Iv = entriedto.PasswordData.Iv,
                    CryptedData = entriedto.PasswordData.CryptedData, Tag = entriedto.PasswordData.Tag, Entrie = null!,
                    Logs = null!
                }
            };
            _dbContext.Set<EncryptedData>().AddRange(encryptedEntries);
            await _dbContext.SaveChangesAsync();

            entry.NameDataId = encryptedEntries[0].IdEncryptedData;
            entry.UserNameDataId = encryptedEntries[1].IdEncryptedData;
            entry.UrlDataId = encryptedEntries[2].IdEncryptedData;
            entry.CommentDataId = encryptedEntries[3].IdEncryptedData;
            entry.PasswordDataId = encryptedEntries[4].IdEncryptedData;
            await _dbContext.SaveChangesAsync();
            
            var logEntry = new Log
            {
                ActionDate = DateTime.UtcNow,
                ActionType = "EntryCreated",
                Details = $"Entrée {entry.IdEntrie} créée dans le vault {vaultId}.",
                UserId = userId,
                VaultId = vaultId,
                EntryId = entry.IdEntrie,

                User = null! 
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
            
            if (!_authenticatorService.IsConnectionValid(userId, vaultId))
                return Unauthorized();
            
            var entries = await _dbContext.Set<Entrie>()
                .Where(e => e.VaultId == vaultId)
                .ToListAsync();

            var result = new List<EntrieDto>();
            foreach (var e in entries)
            {
                var nameData = await _dbContext.Set<EncryptedData>().FindAsync(e.NameDataId);
                var userNameData = await _dbContext.Set<EncryptedData>().FindAsync(e.UserNameDataId);
                var urlData = await _dbContext.Set<EncryptedData>().FindAsync(e.UrlDataId);
                var commentData = await _dbContext.Set<EncryptedData>().FindAsync(e.CommentDataId);
                result.Add(new EntrieDto
                {
                    IdEntrie = e.IdEntrie,
                    VaultId = e.VaultId,
                    CreatedDate = e.CreatedDate,
                    UpdatedDate = e.UpdatedDate,
                    IsDesactivated = e.IsDesactivated,
                    NameData = nameData != null
                        ? new EncryptedDataDto
                        {
                            Iv = nameData.Iv,
                            CryptedData = nameData.CryptedData,
                            Tag = nameData.Tag
                        }
                        : new EncryptedDataDto(),
                    UserNameData = userNameData != null
                        ? new EncryptedDataDto
                        {
                            Iv = userNameData.Iv,
                            CryptedData = userNameData.CryptedData,
                            Tag = userNameData.Tag
                        }
                        : new EncryptedDataDto(),
                    UrlData = urlData != null
                        ? new EncryptedDataDto
                        {
                            Iv = urlData.Iv,
                            CryptedData = urlData.CryptedData,
                            Tag = urlData.Tag
                        }
                        : new EncryptedDataDto(),
                    CommentData = commentData != null
                        ? new EncryptedDataDto
                        {
                            Iv = commentData.Iv,
                            CryptedData = commentData.CryptedData,
                            Tag = commentData.Tag
                        }
                        : new EncryptedDataDto()
                });
            }

            var logListEntity = new Log
            {
                ActionDate = DateTime.UtcNow,
                ActionType = "EntriesListed",
                Details = $"Affichage des entrées du vault {vaultId}.",
                UserId = userId,
                VaultId = vaultId,
                EntryId = null,
                User = null!
            };
            _dbContext.Set<Log>().Add(logListEntity);
            await _dbContext.SaveChangesAsync();

            return Ok(result);
        }


// #if DEBUG
//         [AllowAnonymous]
// #endif
//         [HttpGet("{entryId}/password")]
//         public async Task<IActionResult> GetEntryPassword(int vaultId, int entryId)
//         {
//             var userId = _userService.CurrentUserId;
//             if (userId == 0)
//                 return Unauthorized();
//
//             // Vérifier l'existence et l'appartenance
//             var vault = await _dbContext.Set<Vault>()
//                 .Include(v => v.Users)
//                 .FirstOrDefaultAsync(v => v.IdVault == vaultId);
//             if (vault == null || !vault.Users.Any(u => u.IdUser == userId))
//                 return Unauthorized();
//
//             var entry = await _dbContext.Set<Entrie>()
//                 .FirstOrDefaultAsync(e => e.IdEntrie == entryId && e.VaultId == vaultId);
//             if (entry == null)
//                 return NotFound();
//
//             var pwdData = await _dbContext.Set<EncryptedData>()
//                 .FirstOrDefaultAsync(d => d.IdEncryptedData == entry.NameDataId /* remplacer par PasswordDataId une fois ajouté */);
//             if (pwdData == null)
//                 return NotFound();
//
//             // 4. Journaliser l'affichage du mot de passe
//             var logPwdEntity = new Log
//             {
//                 ActionDate = DateTime.UtcNow,
//                 ActionType = "EntryPasswordViewed",
//                 Details    = $"Affichage du mot de passe pour l'entrée {entryId} du vault {vaultId}.",
//                 UserId     = userId,
//                 VaultId    = vaultId,
//                 EntryId    = entryId,
//                 User       = null!
//             };
//             _dbContext.Set<Log>().Add(logPwdEntity);
//             await _dbContext.SaveChangesAsync();
//
//             var dto = new EntryPasswordDto
//             {
//                 IdEntrie     = entry.IdEntrie,
//                 PasswordData = entry.
//                
//             };
//             return Ok(dto);
//         }
//     }
    }
}

