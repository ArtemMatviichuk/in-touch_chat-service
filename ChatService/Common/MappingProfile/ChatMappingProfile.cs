using AutoMapper;
using ChatService.Common.Dtos.Chat;
using ChatService.Data.Entity;

namespace ChatService.Common.MappingProfile
{
    public class ChatMappingProfile : Profile
    {
        public ChatMappingProfile()
        {
            // CHAT
            CreateMap<Chat, ChatDto>()
                .ForMember(e => e.Participants, opt => opt.MapFrom(e => e.Participants!.Select(e => e.PublicId)))
                .AfterMap(MapInfoFields);

        }

        private static void MapInfoFields(Chat chat, ChatDto dto)
        {
            if (!chat.IsPrivate && chat.Info != null)
            {
                dto.Name = chat.Info.Name;
                dto.Description = chat.Info.Description;
                dto.LastModified = chat.Info.LastModified;
                dto.HasImage = !string.IsNullOrWhiteSpace(chat.Info.ChatImageName);
            }
        }
    }
}
