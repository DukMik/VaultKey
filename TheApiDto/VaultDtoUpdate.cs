namespace TheApiDto;

public class VaultDtoUpdate
{
    public string VaultName { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public byte[] PrivateKey { get; set; } = Array.Empty<byte>();
}