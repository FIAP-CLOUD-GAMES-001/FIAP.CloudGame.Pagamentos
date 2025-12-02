using FIAP.CloudGames.Pagamentos.Domain.Requests;
using FIAP.CloudGames.Pagamentos.Domain.Responses;

namespace FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    }
}
