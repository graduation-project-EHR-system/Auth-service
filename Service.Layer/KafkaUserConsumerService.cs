using System.Text.Json;
using Confluent.Kafka;
using Data.Layer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Layer.Configuration;
using Data.Layer.Dtos;
using UserManagementService.Interfaces;
using System.Text.Json.Serialization;

public class KafkaUserConsumerService : BackgroundService
{
    private readonly ILogger<KafkaUserConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string[] _topics = { "user.created", "user.updated", "user.deleted" };
    private readonly KafkaConfig _kafkaConfig;

    public KafkaUserConsumerService(ILogger<KafkaUserConsumerService> logger, IServiceProvider serviceProvider, KafkaConfig kafkaConfig)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _kafkaConfig = kafkaConfig;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
<<<<<<< HEAD
        await Task.Yield();
=======

        _logger.LogInformation("Started listening to Kafka topics: {Topics}", string.Join(", ", _topics));
>>>>>>> feature/integrate-with-kafka

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaConfig.BootstrapServers,
            GroupId = "user-consumer-debug-" + Guid.NewGuid().ToString(),
            AutoOffsetReset = AutoOffsetReset.Earliest,
            ApiVersionRequest = false,
            BrokerVersionFallback = "0.10.0",
            SocketTimeoutMs = 10000,
            SessionTimeoutMs = 10000,
            ReconnectBackoffMs = 1000,
            EnableAutoCommit = true
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_topics);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("About to consume message from Kafka...");

                var result = consumer.Consume(stoppingToken);

                _logger.LogInformation("Consumed message: {Message}", result.Message?.Value);

                var topic = result.Topic;
                var message = result.Message.Value;

                _logger.LogInformation($"Received message from topic: {topic} on {DateTime.Now}");

                using var scope = _serviceProvider.CreateScope();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                switch (topic)
                {
                    case "user.created":
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }  // CamelCase makes "Male" match enum
                        };
                        var createdUser = JsonSerializer.Deserialize<KafkaUserEvent>(message , options);
                        await userService.HandleUserCreatedEventAsync(createdUser);
                        break;
                        // case KafkaTopic.USER_UPDATED.GetTopicName():
                        //     var updatedUser = JsonSerializer.Deserialize<KafkaUserEvent>(message);
                        //     // await userService.HandleUserUpdatedAsync(updatedUser);
                        //     break;
                        // case KafkaTopic.USER_DELETED.GetTopicName():
                        //     var deletedUser = JsonSerializer.Deserialize<KafkaUserEvent>(message);
                        //     // await userService.HandleUserDeletedAsync(deletedUser);
                        //     break;
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError($"Consume error: {ex.Message}");
            }
        }
    }
}
