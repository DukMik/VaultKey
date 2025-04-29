using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkComm.EfModel.Models;
[Table("Entrie")]
public class Entrie
{
    [Key]
        [Column("IdEntrie"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEntrie { get; set; }
        
        public int VaultId { get; set; }

        public int NameDataId { get; set; }

        public int UserNameDataId { get; set; }

        public int PasswordDataId { get; set; }

        public int UrlDataId { get; set; }

        public int CommentDataId { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public DateTime UpdatedDate { get; set; }

        public bool IsDesactivated { get; set; }
       

        #region navigation
        public required Vault Vault { get; set; }
        public required List<EncryptedData> EncryptedData { get; set;}
        public required List<Log> Logs { get; set; }
        #endregion 


        internal class VaultConfiguration : IEntityTypeConfiguration<Entrie>
        {
            public void Configure(EntityTypeBuilder<Entrie> builder)
            {          

                builder.HasMany(v => v.EncryptedData)
                    .WithOne(e => e.Entrie)
                    .HasForeignKey(f => f.EntrieId);
            }
        }
}