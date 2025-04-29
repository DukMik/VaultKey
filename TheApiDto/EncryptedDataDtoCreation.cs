namespace TheApiDto;

public class CreateEncryptedDataDto
{
    public byte[] Iv { get; set; } = Array.Empty<byte>();
    public byte[] CryptedData { get; set; } = Array.Empty<byte>();
    public byte[] Tag { get; set; } = Array.Empty<byte>();
}