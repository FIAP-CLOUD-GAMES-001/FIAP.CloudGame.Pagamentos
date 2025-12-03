using FIAP.CloudGames.Pagamentos.Domain.Entities;
using FIAP.CloudGames.Pagamentos.Domain.Enums;
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
            var payment = new Payment(
                request.OrderId,
                request.OrderAmount,
                ((PaymentMethod)Convert.ToInt32(request.PaymentMethod)),
                request.OrderDate);

            await _repository.CreateAsync(payment);

            // Notify Azure Functions
            //await _httpClient.PostAsJsonAsync("/api/payment-notification", new
            //{
            //    payment.OrderId,
            //    payment.PaymentMethod,
            //    payment.PaymentStatus
            //});

            return new PaymentResponse
            {
                Id = payment.Id,
                PaymentStatus = payment.PaymentStatus.ToString(),
                ProcessedDate = payment.ProcessedDate.GetValueOrDefault()
            };
        }
    }
}
