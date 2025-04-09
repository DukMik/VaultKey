using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkComm.EfModel.Models;
[Table("Log")]
public class Log
{
        [Key]
        [Column("IdLog"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdLog { get; set; }

        public DateTime ActionDate { get; set; }
        
        [StringLength(255)]
        public string ActionType { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string Details { get; set; } = string.Empty;

        public int UserId { get; set; }

        public int? VaultId { get; set; }

        public int? EntryId { get; set; }

        public int? DataId { get; set; }
        

        [ForeignKey("UserId")]
        public virtual required User User { get; set; }

        [ForeignKey("VaultId")]
        public virtual Vault? Vault { get; set; }

        [ForeignKey("EntryId")]
        public virtual Entrie? Entrie { get; set; }

        [ForeignKey("DataId")]
        public virtual EncryptedData? EncryptedData { get; set; }
}