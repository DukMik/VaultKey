namespace TheApiDto;

public class VaultDtoCreation
{
    public int UserId { get; set; }
    public string VaultName { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string KeyHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public byte[] PrivateKey { get; set; } = Array.Empty<byte>();
}