using ChatService.EventProcessing;

namespace ChatService.EventProcessing.Interfaces
{
    public interface IEventDeterminator
    {
        EventType DetermineEvent(string message);
    }
}
