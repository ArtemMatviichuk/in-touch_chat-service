namespace ChatService.EventProcessing.Interfaces
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string message);
    }
}
