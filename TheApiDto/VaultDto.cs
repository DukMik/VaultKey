namespace TheApiDto;

public class VaultDto
{
    public int IdVault { get; set; }
    public int UserId { get; set; }
    public string VaultName { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public byte[] KeyHash { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>();
    public byte[] PrivateKey { get; set; } = Array.Empty<byte>();
    public bool IsDesactivated { get; set; }
    
    public UserDto? User { get; set; }
    public List<EntrieDto>? Entries { get; set; }
    public List<LogDto>? Logs { get; set; }
}