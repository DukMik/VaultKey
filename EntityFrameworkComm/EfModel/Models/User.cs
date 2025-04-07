using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkComm.EfModel.Models;
[Table("User")]
public class User
{
    [Key][Column("IdUser"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdUser { get; set; }
    
    public Guid EntraIdUser { get; set; }

    public required List<Vault> Vaults { get; set; }
    public required List<Log> AuditLogs { get; set; }
} 

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(u => u.Vaults)
            .WithMany(v => v.Users)
            .UsingEntity(j => j.ToTable("UserVaults"));
    }
}