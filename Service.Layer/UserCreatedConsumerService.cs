using System.Text.Json;
using Confluent.Kafka;
using Data.Layer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Layer.Configuration;
using Service.Layer.DTO;

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
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaConfig.BootstrapServers,
            GroupId = _kafkaConfig.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_topics);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var topic = result.Topic;
                var message = result.Message.Value;

                _logger.LogInformation($"Received message from topic: {topic} on {DateTime.Now}");

                using var scope = _serviceProvider.CreateScope();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                switch (topic)
                {
                    // case "user.created":
                        // var createdUser = JsonSerializer.Deserialize<KafkaUserEvent>(message);
                        // await userService.HandleUserCreatedAsync(createdUser);
                        // break;
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
