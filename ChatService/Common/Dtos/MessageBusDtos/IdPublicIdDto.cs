namespace ChatService.Common.Dtos.MessageBusDtos
{
    public class IdPublicIdDto : IdDto
    {
        public IdPublicIdDto()
            : base()
        {

        }

        public IdPublicIdDto(int id, string publicId)
            : base(id)
        {
            PublicId = publicId;
        }

        public string PublicId { get; set; } = string.Empty;
    }
}
