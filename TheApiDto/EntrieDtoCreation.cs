namespace TheApiDto;

public class EntrieDtoCreation
{
    public bool IsDesactivated { get; set; } = false;
    public CreateEncryptedDataDto NameData { get; set; } = new();
    public CreateEncryptedDataDto UserNameData { get; set; } = new();
    public CreateEncryptedDataDto UrlData { get; set; } = new();
    public CreateEncryptedDataDto CommentData { get; set; } = new();
    public CreateEncryptedDataDto PasswordData { get; set; } = new();
}