namespace Service.Layer.Configuration;

public class KafkaConfig
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string AutoOffsetReset { get; set; } = string.Empty;
    public List<string> Topics { get; set; } = new();
} 