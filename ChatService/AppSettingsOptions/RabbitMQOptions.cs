namespace ChatService.AppSettingsOptions;

public class RabbitMQOptions
{
    public string Host { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string ClientProvidedName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ExchangeOptions Auth { get; set; } = new ExchangeOptions();
}
