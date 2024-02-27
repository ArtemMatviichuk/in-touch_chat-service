using ChatService.Common.Dtos.MessageBusDtos;
using ChatService.EventProcessing.Interfaces;
using System.Text.Json;

namespace ChatService.EventProcessing.Implementations
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IEventDeterminator _eventDeterminator;
        private readonly IEventsService _eventsService;

        private readonly ILogger<EventProcessor> _logger;

        public EventProcessor(IEventDeterminator eventDeterminator, IEventsService eventsService, ILogger<EventProcessor> logger)
        {
            _eventDeterminator = eventDeterminator;
            _eventsService = eventsService;
            _logger = logger;
        }

        public async Task ProcessEvent(string message)
        {
            var eventType = _eventDeterminator.DetermineEvent(message);

            try
            {
                await ExecuteEvent(eventType, message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can not send email: {ex.Message}");
            }
        }

        private Task ExecuteEvent(EventType type, string message)
        {
            return type switch
            {
                EventType.Undetermined => throw new Exception("Event type is not determined"),
                EventType.UserRegistered => _eventsService.CreateUser(Deserialize<IdPublicIdDto>(message)),
                EventType.UserRemoved => _eventsService.ClearUser(Deserialize<IdDto>(message)),
                _ => throw new Exception("Event type does not exist or is not implemented"),
            };
        }

        private static T? Deserialize<T>(string message)
        {
            return JsonSerializer.Deserialize<T>(message);
        }
    }
}

