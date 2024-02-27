using System.Text;
using ChatService.AppSettingsOptions;
using ChatService.EventProcessing.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly RabbitMQOptions _options;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MessageBusSubscriber> _logger;

        private IConnection? _connection;
        private IModel? _chanel;
        private string? _consumerTag;

        public MessageBusSubscriber(RabbitMQOptions options, IServiceScopeFactory scopeFactory,
            ILogger<MessageBusSubscriber> logger)
        {
            _options = options;
            _scopeFactory = scopeFactory;
            _logger = logger;

            InitializeRabbitMQ();
        }

        public override void Dispose()
        {
            if (_chanel != null && _chanel.IsOpen)
            {
                _chanel.BasicCancel(_consumerTag);

                _chanel.Close();
                _connection?.Close();
            }

            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_chanel);
            consumer.Received += async (moduleHandle, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

                        await eventProcessor.ProcessEvent(message);
                        _chanel?.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"RabbitMQ message was not processed properly: {ex.Message}");
                        throw;
                    }
                };

            _consumerTag = _chanel?.BasicConsume(_options.Auth.QueueName, false, consumer: consumer);

            return Task.CompletedTask;
        }

        private void InitializeRabbitMQ()
        {
            Console.WriteLine($"--> RabbitMQ address: {_options.Host}:{_options.Port}");
            var factory = new ConnectionFactory()
            {
                HostName = _options.Host,
                Port = int.Parse(_options.Port),
                ClientProvidedName = _options.ClientProvidedName,
                UserName = _options.UserName,
                Password = _options.Password,
            };

            _connection = factory.CreateConnection();
            _chanel = _connection.CreateModel();

            _chanel.ExchangeDeclare(_options.Auth.Exchange, type: ExchangeType.Fanout);
            _chanel.QueueDeclare(_options.Auth.QueueName, false, false, false, null);
            _chanel.QueueBind(_options.Auth.QueueName, _options.Auth.Exchange, _options.Auth.RoutingKey, null);

            Console.WriteLine("--> Listening on the Message Bus");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("RabbitMQ connection closed");
        }
    }
}