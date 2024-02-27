namespace ChatService.Common.Dtos.Chat
{
    public class CreateChatDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }

        public IEnumerable<string>? ParticipantIds { get; set; }
    }
}
