namespace ChatService.Data.Entity
{
    public class User
    {
        public int Id { get; set; }
        public int? AuthenticationId { get; set; }
        public string PublicId { get; set; } = string.Empty;
    }
}