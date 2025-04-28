namespace TheApiDto;

public class EntrieListDto
{
    public int IdEntrie { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsDesactivated { get; set; }
    public string NameData { get; set; } = string.Empty;
    public string UserNameData { get; set; } = string.Empty;
    public string UrlData { get; set; } = string.Empty;
    public string CommentData { get; set; } = string.Empty;
    public string PasswordData { get; set; } = "";
}