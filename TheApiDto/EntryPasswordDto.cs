namespace TheApiDto
{
    public class EntryPasswordDto
    {
        public int IdEntrie { get; set; }
        public string PasswordData { get; set; } = string.Empty;
        public string Iv { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
    }
}