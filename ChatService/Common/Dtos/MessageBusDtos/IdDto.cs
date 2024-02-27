namespace ChatService.Common.Dtos.MessageBusDtos
{
    public class IdDto
    {
        public IdDto()
        {

        }

        public IdDto(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}