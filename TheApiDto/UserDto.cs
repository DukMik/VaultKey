namespace TheApiDto;

public class UserDto
{
    public int IdUser { get; set; }
    public Guid EntraIdUser { get; set; }
    public List<VaultDto>? Vaults { get; set; }
    public List<LogDto>? Logs { get; set; } 
}