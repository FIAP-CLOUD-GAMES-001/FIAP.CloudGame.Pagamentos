using FIAP.CloudGames.Pagamentos.Domain.Entities;
using FIAP.CloudGames.Pagamentos.Domain.Enums;

namespace FIAP.CloudGames.Pagamentos.Domain.Interfaces.Repositoiries
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(string id);
        Task<Payment> GetByOrderIdAsync(string orderId);
        Task<IEnumerable<Payment>> GetPaymentsByDateAsync(DateTime date);
        Task CreateAsync(Payment payment);
        Task UpdateStatusAsync(string id, PaymentStatus newStatus);
        Task<bool> ExistsAsync(string id);
    }
}
