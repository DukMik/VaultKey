using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkComm.EfModel.Models;

[Table("EncryptedData")]
public class EncryptedData
{
    [Key]
    [Column("IdEncryptedData"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEncryptedData { get; set; }

    public int EntrieId { get; set; }

    public byte[] Iv { get; set; } = Array.Empty<byte>();

    public byte[] CryptedData { get; set; } = Array.Empty<byte>();

    public byte[] Tag { get; set; } = Array.Empty<byte>();
    

    public required Entrie Entrie { get; set; }
    public required List<Log> Logs { get; set; }

}