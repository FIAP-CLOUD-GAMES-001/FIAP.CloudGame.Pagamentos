using FIAP.CloudGames.Pagamentos.Domain.Entities;
using FIAP.CloudGames.Pagamentos.Domain.Enums;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Repositoiries;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services;
using FIAP.CloudGames.Pagamentos.Domain.Requests;
using FIAP.CloudGames.Pagamentos.Domain.Responses;
using System.Net.Http.Json;

namespace FIAP.CloudGames.Pagamentos.Service.Services;

public class PaymentService(IPaymentRepository repository, IHttpClientFactory clientFactory) : IPaymentService
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
            ((PaymentMethod)Convert.ToInt32(request.PaymentMethod)),
            request.OrderDate);

        payment.Approve();
        await repository.CreateAsync(payment);

        var httpClient = clientFactory.CreateClient();

        await httpClient.PostAsJsonAsync($"/api/webhook/payment", new
        {
            payment.OrderId,
            PaymentMethod = (int)payment.PaymentMethod,
            OrderAmount = payment.OrderAmount,
            PaymentStatus = (int)payment.PaymentStatus,
            ProcessedDate = payment.ProcessedDate
        });

        return new PaymentResponse
        {
            Id = payment.Id,
            PaymentStatus = Convert.ToInt32(payment.PaymentStatus).ToString(),
            ProcessedDate = payment.ProcessedDate.GetValueOrDefault()
        };
    }
}
