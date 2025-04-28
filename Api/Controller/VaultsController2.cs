using System.Security.Claims;
using EntityFrameworkComm.EfModel.Context;
using Api.Service;
using EntityFrameworkComm.EfModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheApiDto;

namespace Api.Controller;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class VaultsController2 : ControllerBase
{
    private readonly Context _context;

    public VaultsController2(Context context)
    {
        _context = context;
    }

    /// <summary>
    /// Crée un nouveau vault pour l'utilisateur connecté.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateVault([FromBody] VaultDto vaultDto)
    {
        // 1. Récupérer l'ID de l'utilisateur connecté depuis les claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized("Impossible de déterminer l'utilisateur connecté.");

        // 2. Vérifier que l'utilisateur existe en base
        var user = await _context.User.FindAsync(userId);
        if (user == null)
            return BadRequest($"Utilisateur avec ID {userId} introuvable.");

        // 3. Mapper le DTO vers l'entité EF
        var vaultEntity = new Vault
        {
            UserId         = userId,
            VaultName      = vaultDto.VaultName,
            DateCreated    = DateTime.UtcNow,
            KeyHash        = vaultDto.KeyHash,
            Salt           = vaultDto.Salt,
            PrivateKey     = vaultDto.PrivateKey,
            IsDesactivated = vaultDto.IsDesactivated,

            // <-- Obligatoire depuis C# 11 pour les propriétés required
            Users   = new List<User> { user },
            Entries = new List<Entrie>(),
            Logs    = new List<Log>()
        };

        // 4. Persister
        _context.Vault.Add(vaultEntity);
        await _context.SaveChangesAsync();

        // 5. Préparer le DTO de retour
        vaultDto.IdVault = vaultEntity.IdVault;
        vaultDto.UserId = vaultEntity.UserId;
        vaultDto.DateCreated = vaultEntity.DateCreated;
        // Ne pas oublier de remplir éventuellement la propriété User si besoin

        // 6. Retourner 201 Created
        return CreatedAtAction(
            nameof(GetVaultById),
            new { id = vaultEntity.IdVault },
            vaultDto
        );
    }

    /// <summary>
    /// Récupère un vault par son ID (utile pour le CreatedAtAction).
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVaultById(int id)
    {
        var vault = await _context.Vault
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.IdVault == id);

        if (vault == null)
            return NotFound();

        var dto = new VaultDto
        {
            IdVault = vault.IdVault,
            UserId = vault.UserId,
            VaultName = vault.VaultName,
            DateCreated = vault.DateCreated,
            KeyHash = vault.KeyHash,
            Salt = vault.Salt,
            PrivateKey = vault.PrivateKey,
            IsDesactivated = vault.IsDesactivated
            // Ne pas oublier de peupler User et Entries/Logs si nécessaire
        };

        return Ok(dto);
    }
}

