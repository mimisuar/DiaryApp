namespace Diary.Server.Data
{
    public class JournalEntry
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string EncryptedBody { get; set; } = default!;
        public DateTime CreatedOn { get; set; }

        public User Creator { get; set; } = default!;
        public string CreatorId { get; set; } = "";
    }
}
