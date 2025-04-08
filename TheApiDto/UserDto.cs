namespace TheApiDto;

public class UserDto
{
    public int IdUser { get; set; }
    public Guid EntraIdUser { get; set; }
    public required List<VaultDto> Vaults { get; set; }
    public required List<LogDto> AuditLogs { get; set; }
}