using FIAP.CloudGames.Pagamentos.Domain.Entities;
using FIAP.CloudGames.Pagamentos.Domain.Enums;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Repositoiries;
using MongoDB.Driver;

namespace FIAP.CloudGames.Pagamentos.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{

    private readonly IMongoCollection<Payment> _collection;

    public PaymentRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Payment>("payments");
    }

    public async Task<Payment?> GetByIdAsync(string id)
    {
        var filter = Builders<Payment>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Payment> GetByOrderIdAsync(string orderId)
    {
        var filter = Builders<Payment>.Filter.Eq(x => x.OrderId, orderId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByDateAsync(DateTime date)
    {
        var filter = Builders<Payment>.Filter.Eq(x => x.OrderDate, date);
        return await _collection.Find(filter).ToListAsync();
    }


    public async Task CreateAsync(Payment payment)
    {
        await _collection.InsertOneAsync(payment);
    }

    public async Task UpdateStatusAsync(string id, EPaymentStatus newStatus)
    {
        var update = Builders<Payment>.Update
            .Set(x => x.PaymentStatus, newStatus)
            .Set(x => x.ProcessedDate, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(
            Builders<Payment>.Filter.Eq(x => x.Id, id),
            update
        );

        if (result.MatchedCount == 0)
            throw new KeyNotFoundException($"Payment '{id}' not found.");
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var filter = Builders<Payment>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).AnyAsync();
    }

}
