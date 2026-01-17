using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services;
using FIAP.CloudGames.Pagamentos.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FIAP.CloudGames.Pagamentos.Service.Services;

public class RabbitMqService : IRabbitMqService, IDisposable
{
    private readonly ILogger<RabbitMqService> _logger;
    private readonly RabbitMqSettings _settings;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService(ILogger<RabbitMqService> logger, IOptions<RabbitMqSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                SocketReadTimeout = TimeSpan.FromSeconds(30),
                SocketWriteTimeout = TimeSpan.FromSeconds(30)
            };

            _logger.LogInformation("Attempting to connect to RabbitMQ at {Host}:{Port} with user {Username}",
                _settings.Host, _settings.Port, _settings.Username);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _logger.LogInformation("Successfully connected to RabbitMQ");

            SetupRabbitMqInfrastructure();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ at {Host}:{Port}. Please ensure RabbitMQ is running and accessible.",
                _settings.Host, _settings.Port);
            throw;
        }
    }

    private void SetupRabbitMqInfrastructure()
    {
        try
        {
            _channel.ExchangeDeclare(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            _logger.LogInformation("Exchange '{Exchange}' declared successfully", _settings.ExchangeName);

            _channel.QueueDeclare(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation("Queue '{Queue}' declared successfully", _settings.QueueName);

            var retryArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _settings.ExchangeName },
                { "x-dead-letter-routing-key", _settings.RoutingKey },
                { "x-message-ttl", 30000 }
            };

            _channel.QueueDeclare(
                queue: _settings.RetryQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: retryArgs);

            _logger.LogInformation("Retry queue '{Queue}' declared successfully with TTL 30s", _settings.RetryQueueName);

            _channel.QueueDeclare(
                queue: _settings.FailQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation("Fail queue '{Queue}' declared successfully", _settings.FailQueueName);

            _channel.QueueBind(
                queue: _settings.QueueName,
                exchange: _settings.ExchangeName,
                routingKey: _settings.RoutingKey);

            _logger.LogInformation("Queue '{Queue}' bound to exchange '{Exchange}' with routing key '{RoutingKey}'",
                _settings.QueueName, _settings.ExchangeName, _settings.RoutingKey);

            _logger.LogInformation("RabbitMQ infrastructure configured successfully");
        }
        catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex) when (ex.Message.Contains("PRECONDITION_FAILED"))
        {
            _logger.LogError(ex,
                "Failed to declare queues. The queues may already exist with different configurations. " +
                "Please delete the existing queues from RabbitMQ management interface (http://localhost:15672) and restart the application. " +
                "Queues to delete: {Queues}",
                string.Join(", ", _settings.QueueName, _settings.RetryQueueName, _settings.FailQueueName));
            throw new InvalidOperationException(
                "RabbitMQ queues already exist with incompatible configuration. " +
                "Please delete the queues manually from RabbitMQ management interface and restart the application.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup RabbitMQ infrastructure");
            throw;
        }
    }

    public Task PublishPaymentNotificationAsync(object message)
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: _settings.ExchangeName,
                routingKey: _settings.RoutingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Payment notification published to RabbitMQ. Message: {Message}", jsonMessage);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing payment notification to RabbitMQ");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}