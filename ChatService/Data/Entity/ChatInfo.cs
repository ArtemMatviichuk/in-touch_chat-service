namespace ChatService.Data.Entity
{
    public class ChatInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ChatImageName { get; set; }
        public DateTime? LastModified { get; set; }

        public int ChatId { get; set; }
        public Chat? Chat { get; set; }
    }
}
