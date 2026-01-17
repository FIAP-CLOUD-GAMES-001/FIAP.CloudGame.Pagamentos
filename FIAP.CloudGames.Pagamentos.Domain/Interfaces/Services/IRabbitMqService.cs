namespace FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services;

public interface IRabbitMqService
{
    Task PublishPaymentNotificationAsync(object message);
}