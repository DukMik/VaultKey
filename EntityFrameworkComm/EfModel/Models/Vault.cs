using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkComm.EfModel.Models;

public class Vault
{
    [Key][Column("IdVault"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdVault { get; set; }

    public int UserId { get; set; }

    [MaxLength(255)]
    public string VaultName { get; set; } = string.Empty;

    public DateTime DateCreated { get; set; }

    [MaxLength(255)]
    public string KeyHash { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Salt { get; set; } = string.Empty;

    public byte[] PrivateKey { get; set; } = Array.Empty<byte>();

    public bool IsDesactivated { get; set; }
    
    public required List<User> Users { get; set; }
    public required List<Entrie> Entries { get; set; }
    public required List<Log> AuditLogs { get; set; }
}


internal class VaultConfiguration : IEntityTypeConfiguration<Vault>
{
    public void Configure(EntityTypeBuilder<Vault> builder)
    {
        builder.HasMany(v => v.Users)
            .WithMany(u => u.Vaults)
            .UsingEntity(j => j.ToTable("UserVaults"));

        builder.HasMany(v => v.Entries)
            .WithOne(e => e.Vault)
            .HasForeignKey(f => f.VaultId);
    }
}
