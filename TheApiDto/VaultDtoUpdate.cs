namespace TheApiDto;

public class VaultDtoUpdate
{
    public string VaultName { get; set; } = string.Empty;
    public byte[] KeyHash { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>();
    public byte[] PrivateKey { get; set; } = Array.Empty<byte>();
}