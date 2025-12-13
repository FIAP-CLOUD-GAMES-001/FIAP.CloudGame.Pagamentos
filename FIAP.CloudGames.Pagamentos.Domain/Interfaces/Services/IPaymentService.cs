using FIAP.CloudGames.Pagamentos.Domain.Entities;
using FIAP.CloudGames.Pagamentos.Domain.Requests;
using FIAP.CloudGames.Pagamentos.Domain.Responses;

namespace FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
        Task<IEnumerable<Payment>> GetPaymentsByDateAsync(DateTime date);
        Task<Payment?> GetPaymentByOrderIdAsync(string orderId);
    }
}
