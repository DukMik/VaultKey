namespace TheApiDto;

public class EntrieDto
{
    public int IdEntrie { get; set; }
    public int VaultId { get; set; }
    public required EncryptedDataDto NameData { get; set; }
    public required EncryptedDataDto UserNameData { get; set; }       
    public required EncryptedDataDto UrlData { get; set; }
    public required EncryptedDataDto CommentData { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsDesactivated { get; set; }
}