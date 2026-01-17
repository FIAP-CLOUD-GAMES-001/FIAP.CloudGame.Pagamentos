namespace FIAP.CloudGames.Pagamentos.Domain.Models;

public class RabbitMqSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public string RetryQueueName { get; set; } = string.Empty;
    public string FailQueueName { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
}