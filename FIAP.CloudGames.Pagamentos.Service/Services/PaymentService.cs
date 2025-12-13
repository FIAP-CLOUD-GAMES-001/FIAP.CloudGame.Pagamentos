using FIAP.CloudGames.Pagamentos.Domain.Entities;
using FIAP.CloudGames.Pagamentos.Domain.Enums;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Repositoiries;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services;
using FIAP.CloudGames.Pagamentos.Domain.Requests;
using FIAP.CloudGames.Pagamentos.Domain.Responses;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace FIAP.CloudGames.Pagamentos.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly HttpClient _httpClient;
        private readonly string _azureFunctionsBaseUrl;

        public PaymentService(IPaymentRepository repository, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _repository = repository;
            _httpClient = clientFactory.CreateClient("NotificationClient");

            _azureFunctionsBaseUrl = configuration["AzureFunctions:BaseUrl"];
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(string orderId)
        {
            return await _repository.GetByOrderIdAsync(orderId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateAsync(DateTime date)
        {
            return await _repository.GetPaymentsByDateAsync(date);
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            var payment = new Payment(
                request.OrderId,
                request.OrderAmount,
                ((PaymentMethod)Convert.ToInt32(request.PaymentMethod)),
                request.OrderDate);

            payment.Approve();
            await _repository.CreateAsync(payment);

            var functionUrl = $"{_azureFunctionsBaseUrl}/api/webhook/payment";
            await _httpClient.PostAsJsonAsync(functionUrl, new
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
}
