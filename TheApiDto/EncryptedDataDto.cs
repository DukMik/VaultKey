namespace TheApiDto;

public class EncryptedDataDto
{
    public int IdEncryptedData { get; set; }
    public int EntrieId { get; set; }
    public byte[] Iv { get; set; } = Array.Empty<byte>();
    public byte[] CryptedData { get; set; } = Array.Empty<byte>();
    public byte[] Tag { get; set; } = Array.Empty<byte>();
    public EntrieDto? Entrie { get; set; }
    public List<LogDto>? Logs { get; set; }
}