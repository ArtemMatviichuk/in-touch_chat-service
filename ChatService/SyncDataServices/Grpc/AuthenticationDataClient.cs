using AuthService;
using ChatService.Common.Constants;
using ChatService.Common.Dtos.MessageBusDtos;
using Grpc.Net.Client;

namespace ChatService.SyncDataServices.Grpc
{
    public class AuthenticationDataClient : IAuthenticationDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationDataClient> _logger;

        public AuthenticationDataClient(IConfiguration configuration, ILogger<AuthenticationDataClient> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IEnumerable<IdPublicIdDto>?> GetAllUsers()
        {
            Console.WriteLine($"--> Calling gRPC Service {_configuration[AppConstants.GrpcAuthentication]}");
            _logger.LogInformation($"Calling gRPC Service {_configuration[AppConstants.GrpcAuthentication]}");

            var channel = GrpcChannel.ForAddress(_configuration[AppConstants.GrpcAuthentication]!);
            var client = new GrpcUsers.GrpcUsersClient(channel);
            var request = new GetAllRequest();

            try
            {
                var replay = await client.GetAllUsersAsync(request);
                return replay.Users.Select(u => new IdPublicIdDto(u.UserId, u.PublicId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call gRPC Server: {ex.Message}\n{ex.InnerException?.Message}");
                _logger.LogInformation($"Could not call gRPC Server: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }
    }
}