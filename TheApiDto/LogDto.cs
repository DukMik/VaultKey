namespace TheApiDto;

public class LogDto
{
    public int IdAuditLog { get; set; }
    public DateTime ActionDate { get; set; }
    public string ActionType { get; set; } = String.Empty;
    public string Details { get; set; } = String.Empty;
    public int UserId { get; set; }
    public int? VaultId { get; set; }
    public int? EntryId { get; set; }
    public int? DataId { get; set; }
}