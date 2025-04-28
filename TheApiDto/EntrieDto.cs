namespace TheApiDto;

public class EntrieDto
{
    public int IdEntrie { get; set; }
    public int VaultId { get; set; }
    public int NameDataId { get; set; }
    public int UserNameDataId { get; set; }       
    public int UrlDataId { get; set; }
    public int CommentDataId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsDesactivated { get; set; }
}