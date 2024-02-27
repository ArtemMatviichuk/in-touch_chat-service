using System.Text.Json;
using ChatService.Common.Dtos.MessageBusDtos;
using ChatService.EventProcessing;
using ChatService.EventProcessing.Interfaces;

namespace ChatService.EventProcessing.Implementations
{
    public class EventDeterminator : IEventDeterminator
    {
        private readonly Dictionary<string, EventType> _eventsDictionary = new()
        {
            { "User_Registered", EventType.UserRegistered },
            { "User_Removed", EventType.UserRemoved },
        };

        public EventType DetermineEvent(string message)
        {
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(message);

            if (string.IsNullOrWhiteSpace(eventType?.Event))
                return EventType.Undetermined;

            if (!_eventsDictionary.TryGetValue(eventType.Event, out EventType value))
                return EventType.Undetermined;

            return value;
        }
    }
}
