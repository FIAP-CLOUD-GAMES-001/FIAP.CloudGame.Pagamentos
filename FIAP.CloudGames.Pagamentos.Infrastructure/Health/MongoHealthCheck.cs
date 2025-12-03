using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace FIAP.CloudGames.Pagamentos.Infrastructure.Health
{
    public class MongoHealthCheck : IHealthCheck
    {
        private readonly IMongoClient _client;

        public MongoHealthCheck(IMongoClient client)
        {
            _client = client;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Apenas tenta listar bancos, isso garante que a conexão está funcional
                await _client.ListDatabaseNamesAsync(cancellationToken);

                return HealthCheckResult.Healthy("MongoDB is available.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("MongoDB is unavailable.", ex);
            }
        }
    }
}
