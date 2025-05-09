using Confluent.Kafka;
using Data.Layer.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AuthServiceConsumer;

public class KafkaUserConsumer : BackgroundService
{
    private readonly ILogger<KafkaUserConsumer> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topic;

    public KafkaUserConsumer(ILogger<KafkaUserConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false // للتحكم اليدوي في الـ Commit
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _topic = configuration["Kafka:Topic"];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer started.");

        _consumer.Subscribe(_topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(stoppingToken);
                var message = result.Message.Value;

                var userDto = JsonSerializer.Deserialize<UserDto>(message);
                if (userDto != null)
                {
                    await AddUserToSystem(userDto);
                    _consumer.Commit(result); // Commit بعد المعالجة
                    _logger.LogInformation($"Processed user: {userDto.FirstName} {userDto.LastName}");
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError($"Kafka consume error: {ex.Error.Reason}");
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON deserialization error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
            }
        }
    }

    private Task AddUserToSystem(UserDto user)
    {
        
        _logger.LogInformation($"Adding user: {user.FirstName} {user.LastName} with role {user.Role}");
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}