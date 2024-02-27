namespace ChatService.Data.Entity
{
    public class Chat
    {
        public int Id { get; set; }
        public bool IsPrivate { get; set; }

        public DateTime CreatedDate { get; set; }

        public ICollection<User>? Participants { get; set; }
        public ChatInfo? Info { get; set; } // only group chats
    }
}
