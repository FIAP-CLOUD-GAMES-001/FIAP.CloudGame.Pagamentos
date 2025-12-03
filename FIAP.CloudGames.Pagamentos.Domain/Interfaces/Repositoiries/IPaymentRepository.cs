using FIAP.CloudGames.Pagamentos.Domain.Entities;
using FIAP.CloudGames.Pagamentos.Domain.Enums;

namespace FIAP.CloudGames.Pagamentos.Domain.Interfaces.Repositoiries
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(string id);
        Task<IEnumerable<Payment>> GetByOrderIdAsync(string orderId);
        Task CreateAsync(Payment payment);
        Task UpdateStatusAsync(string id, PaymentStatus newStatus);
        Task<bool> ExistsAsync(string id);
    }
}
