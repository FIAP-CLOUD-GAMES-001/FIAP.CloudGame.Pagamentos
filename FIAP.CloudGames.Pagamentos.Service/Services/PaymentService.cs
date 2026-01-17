using FIAP.CloudGames.Pagamentos.Domain.Entities;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Repositoiries;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services;
using FIAP.CloudGames.Pagamentos.Domain.Requests;
using FIAP.CloudGames.Pagamentos.Domain.Responses;
using Microsoft.Extensions.Logging;

namespace FIAP.CloudGames.Pagamentos.Service.Services;

public class PaymentService(
    ILogger<PaymentService> logger,
    IPaymentRepository repository,
    IRabbitMqService rabbitMqService) : IPaymentService
{
    public async Task<Payment?> GetPaymentByOrderIdAsync(string orderId)
    {
        return await repository.GetByOrderIdAsync(orderId);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByDateAsync(DateTime date)
    {
        return await repository.GetPaymentsByDateAsync(date);
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        var payment = new Payment(
            request.OrderId,
            request.OrderAmount,
            request.PaymentMethod,
            request.OrderDate);

        payment.Approve();
        await repository.CreateAsync(payment);

        try
        {
            await rabbitMqService.PublishPaymentNotificationAsync(new
            {
                OrderId = payment.OrderId,
                PaymentId = payment.PaymentId,
                Amount = payment.OrderAmount,
                Status = (int)payment.PaymentStatus,
                PaymentDate = payment.ProcessedDate
            });

            logger.LogInformation("Payment notification sent to RabbitMQ for OrderId: {OrderId}", payment.OrderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send payment notification to RabbitMQ for OrderId: {OrderId}", payment.OrderId);
        }

        return new PaymentResponse
        {
            Id = payment.Id,
            PaymentStatus = Convert.ToInt32(payment.PaymentStatus).ToString(),
            ProcessedDate = payment.ProcessedDate.GetValueOrDefault()
        };
    }
}