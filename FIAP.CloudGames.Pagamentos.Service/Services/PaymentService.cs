using FIAP.CloudGames.Pagamentos.Domain.Entities;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Repositoiries;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services;
using FIAP.CloudGames.Pagamentos.Domain.Requests;
using FIAP.CloudGames.Pagamentos.Domain.Responses;
using System.Net.Http.Json;

namespace FIAP.CloudGames.Pagamentos.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly HttpClient _httpClient;

        public PaymentService(IPaymentRepository repository, IHttpClientFactory clientFactory)
        {
            _repository = repository;
            _httpClient = clientFactory.CreateClient("NotificationClient");
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            var payment = new Payment
            {
                PaymentId = Guid.NewGuid().ToString(),
                OrderId = request.OrderId,
                OrderAmount = request.OrderAmount,
                PaymentMethod = request.PaymentMethod,
                OrderDate = request.OrderDate,
                ProcessedDate = DateTime.UtcNow,
                PaymentStatus = "APPROVED"
            };

            await _repository.InsertAsync(payment);

            // Notify Azure Functions
            await _httpClient.PostAsJsonAsync("/api/payment-notification", new
            {
                payment.OrderId,
                payment.PaymentId,
                payment.PaymentStatus
            });

            return new PaymentResponse
            {
                PaymentId = payment.PaymentId,
                PaymentStatus = payment.PaymentStatus,
                ProcessedDate = payment.ProcessedDate
            };
        }
    }
}
