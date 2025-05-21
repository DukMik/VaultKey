namespace TheApiDto;

public class VaultDtoUpdate
{
    public string VaultName { get; set; } = string.Empty;
    public Byte[] KeyHash { get; set; } = Array.Empty<byte>();
    public Byte[] Salt { get; set; } = Array.Empty<byte>();
    public byte[] PrivateKey { get; set; } = Array.Empty<byte>();
}