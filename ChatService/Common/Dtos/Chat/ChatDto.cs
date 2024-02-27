namespace ChatService.Common.Dtos.Chat
{
    public class ChatDto
    {
        public int Id { get; set; }
        public bool IsPrivate { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool HasImage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModified { get; set; }

        public IEnumerable<string>? Participants { get; set; }
    }
}
