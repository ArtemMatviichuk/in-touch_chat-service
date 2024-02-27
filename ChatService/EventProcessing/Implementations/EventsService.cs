using ChatService.Common.Dtos.MessageBusDtos;
using ChatService.Data.Entity;
using ChatService.Data.Repositories.Interfaces;
using ChatService.EventProcessing.Interfaces;

namespace ChatService.EventProcessing.Implementations
{
    public class EventsService : IEventsService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<EventsService> _logger;

        public EventsService(IUserRepository userRepository, ILogger<EventsService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task CreateUser(IdPublicIdDto? dto)
        {
            if (dto == null)
            {
                string message = "Received DTO object is null";
                _logger.LogError(message);

                throw new Exception(message);
            }

            if (!await _userRepository.Exists(e => e.AuthenticationId == dto.Id))
            {
                await _userRepository.Add(new User() { AuthenticationId = dto.Id, PublicId = dto.PublicId });
                await _userRepository.SaveChanges();
            }
        }

        public async Task ClearUser(IdDto? dto)
        {
            if (dto == null)
            {
                string message = "Received DTO object is null";
                _logger.LogError(message);

                throw new Exception(message);
            }

            var profile = await _userRepository.GetByAuthIdAsTracking(dto.Id);
            if (profile == null)
            {
                _logger.LogInformation($"Profile with authentication ID {dto.Id} not found");
                return;
            }

            profile.AuthenticationId = null;

            await _userRepository.SaveChanges();
        }

        public async Task ClearNotExistingUsers(IEnumerable<IdDto> dtos)
        {
            var profileIds = dtos.Select(p => p.Id);
            var profilesToRemove = await _userRepository.GetAll(e => e.AuthenticationId.HasValue && !profileIds.Contains(e.AuthenticationId.Value));

            foreach (var profile in profilesToRemove)
            {
                await ClearUser(new IdDto(profile.AuthenticationId!.Value));
            }
        }
    }
}
