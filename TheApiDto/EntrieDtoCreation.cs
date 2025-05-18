namespace TheApiDto;

public class EntrieDtoCreation
{
    
    public bool IsDesactivated { get; set; } = false;
    public EncryptedDataDtoCreation NameData { get; set; } = new();
    public EncryptedDataDtoCreation UserNameData { get; set; } = new();
    public EncryptedDataDtoCreation UrlData { get; set; } = new();
    public EncryptedDataDtoCreation CommentData { get; set; } = new();
    public EncryptedDataDtoCreation PasswordData { get; set; } = new();
}