using FIAP.CloudGames.Pagamentos.Domain.Entities;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Repositoiries;
using FIAP.CloudGames.Pagamentos.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FIAP.CloudGames.Pagamentos.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly IMongoCollection<Payment> _collection;

    public PaymentRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var db = client.GetDatabase(settings.Value.Database);
        _collection = db.GetCollection<Payment>(settings.Value.Collection);
    }

    public async Task InsertAsync(Payment payment)
    {
        await _collection.InsertOneAsync(payment);
    }
}