using FIAP.CloudGames.Pagamentos.Domain.Entities;

namespace FIAP.CloudGames.Pagamentos.Domain.Interfaces.Repositoiries
{
    public interface IPaymentRepository
    {
        Task InsertAsync(Payment payment);
    }
}
