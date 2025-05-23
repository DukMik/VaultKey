namespace TheApiDto;

public class VaultDtoCreation
{
    public int UserId { get; set; }
    public string VaultName { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public byte[] KeyHash { get; set; }  = Array.Empty<byte>();
    public byte[] Salt { get; set; }  = Array.Empty<byte>();
    public byte[] PrivateKey { get; set; } = Array.Empty<byte>();
}