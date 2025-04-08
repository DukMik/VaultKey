namespace TheApiDto;

public class EncryptedDataDto
{
    private int IdEncryptedData { get; set; }
    private int EntrieId { get; set; }
    private byte[] Iv { get; set; } = Array.Empty<byte>();
    private byte[] CryptedData { get; set; } = Array.Empty<byte>();
    private byte[] Tag { get; set; } = Array.Empty<byte>();
    public required EntrieDto Entrie { get; set; }
    public required List<LogDto> AuditLogs { get; set; }
}