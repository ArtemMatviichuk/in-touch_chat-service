namespace ChatService.Common.Dtos.Chat
{
    public class UpdateChatDto : CreateChatDto
    {
        public bool RemoveImage { get; set; }
    }
}
