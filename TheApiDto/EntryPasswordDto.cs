namespace TheApiDto
{
    public class EntryPasswordDto
    {
        public int IdEntrie { get; set; }
        public string Name { get; set; } = "";
        public required EncryptedDataDto PasswordData { get; set; }
    }
}