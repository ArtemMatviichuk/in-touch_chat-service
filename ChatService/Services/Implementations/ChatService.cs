using AutoMapper;
using ChatService.Common.Constants;
using ChatService.Common.Dtos.Chat;
using ChatService.Common.Dtos.General;
using ChatService.Common.Exceptions;
using ChatService.Data.Entity;
using ChatService.Data.Repositories.Interfaces;
using ChatService.Services.Interfaces;

namespace ChatService.Services.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IFilesService _filesService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private readonly ILogger<ChatService> _logger;

        public ChatService(IUserRepository userRepository, IChatRepository chatRepository,
            IMessageRepository messageRepository, IFilesService filesService, IMapper mapper,
            IConfiguration configuration, ILogger<ChatService> logger)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _filesService = filesService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IEnumerable<ChatDto>> GetChats(int authId)
        {
            var user = await _userRepository.GetByAuthId(authId);
            if (user == null)
                throw new NotFoundException("User not found");

            var chats = await _chatRepository.GetByUserId(user.Id);
            var dtos = _mapper.Map<IEnumerable<ChatDto>>(chats);

            return dtos;
        }

        public async Task<ChatDto> GetChat(int authId, int chatId)
        {
            var user = await _userRepository.GetByAuthId(authId);
            if (user == null)
                throw new NotFoundException("User not found");

            var chat = await _chatRepository.GetFullChat(chatId);
            if (chat == null)
                throw new NotFoundException("Chat not found");

            if (!chat.Participants!.Any(p => p.Id == user.Id))
                throw new AccessDeniedException("You are not a participant to access this chat");

            var dto = _mapper.Map<ChatDto>(chat);
            return dto;
        }

        public async Task<ChatDto> CreateChat(int authId, CreateChatDto dto)
        {
            var user = await _userRepository.GetByAuthId(authId);
            if (user == null)
                throw new NotFoundException("User not found");

            dto.ParticipantIds ??= new List<string>();
            if (!dto.ParticipantIds.Contains(user.PublicId))
            {
                dto.ParticipantIds = dto.ParticipantIds.Append(user.PublicId);
            }

            var participants = await _userRepository.GetByPublicId(dto.ParticipantIds);
            var chat = new Chat()
            {
                IsPrivate = false,
                CreatedDate = DateTime.Now,
                Participants = participants.ToList(),
                Info = new ChatInfo()
                {
                    Name = dto.Name,
                    Description = dto.Description,
                },
            };

            if (dto.Image != null)
            {
                string[] supportedTypes = _configuration[AppConstants.SupportedImageTypes]!.Split(",");
                if (!supportedTypes.Any(t => dto.Image.FileName.ToLower().EndsWith(t)))
                {
                    throw new ValidationException($"Unsupported type. Only {string.Join(", ", supportedTypes)} types are available");
                }

                chat.Info.ChatImageName = await _filesService.SaveFile(_configuration[AppConstants.FilesPath]!, dto.Image);
            }

            await _chatRepository.Add(chat);
            await _chatRepository.SaveChanges();

            return await GetChat(authId, chat.Id);
        }

        public async Task UpdateChat(int authId, int chatId, UpdateChatDto dto)
        {
            var user = await _userRepository.GetByAuthId(authId);
            if (user == null)
                throw new NotFoundException("User not found");

            var chat = await _chatRepository.GetFullChatAsTracking(chatId);
            if (chat == null)
                throw new NotFoundException("Chat not found");

            if (chat.IsPrivate)
                throw new AccessDeniedException("Private chat can not be edited");

            dto.ParticipantIds ??= new List<string>();
            if (!dto.ParticipantIds.Contains(user.PublicId))
            {
                dto.ParticipantIds = dto.ParticipantIds.Append(user.PublicId);
            }

            var participants = await _userRepository.GetByPublicId(dto.ParticipantIds);
            chat.Participants = participants.ToList();

            if (chat.Info == null)
                chat.Info = new ChatInfo();

            chat.Info.Name = dto.Name;
            chat.Info.Description = dto.Description;
            chat.Info.LastModified = DateTime.Now;

            if (dto.Image != null)
            {
                string[] supportedTypes = _configuration[AppConstants.SupportedImageTypes]!.Split(",");
                if (!supportedTypes.Any(t => dto.Image.FileName.ToLower().EndsWith(t)))
                {
                    throw new ValidationException($"Unsupported type. Only {string.Join(", ", supportedTypes)} types are available");
                }

                RemoveFile(chat.Info.ChatImageName);

                chat.Info.ChatImageName = await _filesService.SaveFile(_configuration[AppConstants.FilesPath]!, dto.Image);
            }
            else if (dto.RemoveImage)
            {
                RemoveFile(chat.Info.ChatImageName);
                chat.Info.ChatImageName = null;
            }

            await _chatRepository.SaveChanges();
        }

        public async Task DeleteChat(int authId, int chatId)
        {
            var user = await _userRepository.GetByAuthId(authId);
            if (user == null)
                throw new NotFoundException("User not found");

            var chat = await _chatRepository.GetFullChat(chatId);
            if (chat == null)
                throw new NotFoundException("Chat not found");

            if (!chat.Participants!.Any(e => e.Id == user.Id))
                throw new AccessDeniedException("You can delete only chats you are in");

            RemoveFile(chat.Info?.ChatImageName);

            _chatRepository.Remove(chat);
            await _chatRepository.SaveChanges();
        }

        public async Task LeaveChat(int authId, int chatId)
        {
            var user = await _userRepository.GetByAuthId(authId);
            if (user == null)
                throw new NotFoundException("User not found");

            var chat = await _chatRepository.GetFullChatAsTracking(chatId);
            if (chat == null)
                throw new NotFoundException("Chat not found");

            if (chat.IsPrivate)
                throw new ValidationException("You can not leave private chat. Try to delete it");

            if (!chat.Participants!.Any(e => e.Id == user.Id))
                throw new ValidationException("You are not participant");

            if (chat.Participants!.Count() == 1)
                throw new ValidationException("You can not leave chat because you are the last person. Try to delete chat");

            chat.Participants = chat.Participants!.Where(e => e.Id != user.Id).ToList();

            await _chatRepository.SaveChanges();
        }

        public async Task<FileDto> GetChatImage(int authId, int chatId)
        {
            var user = await _userRepository.GetByAuthId(authId);
            if (user == null)
                throw new NotFoundException("User not found");

            var chat = await _chatRepository.GetFullChat(chatId);
            if (chat == null)
                throw new NotFoundException("Chat not found");

            if (!chat.Participants!.Any(e => e.Id == user.Id))
                throw new ValidationException("You are not participant");

            if (chat.IsPrivate)
                throw new ValidationException("Private chat does not have image");

            if (chat.Info == null)
                throw new ValidationException("Chat does not have additional information");

            if (string.IsNullOrWhiteSpace(chat.Info.ChatImageName))
                throw new ValidationException("Chat does not have image");

            var fileDto = await _filesService.GetFile(_configuration[AppConstants.FilesPath]!, chat.Info.ChatImageName);
            if (fileDto == null || fileDto.Bytes == null)
                throw new NotFoundException("Image not found");

            return fileDto;
        }

        private void RemoveFile(string? fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                File.Delete(Path.Combine(_configuration[AppConstants.FilesPath]!, fileName));
            }
        }
    }
}
