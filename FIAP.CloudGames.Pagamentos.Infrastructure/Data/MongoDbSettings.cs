namespace FIAP.CloudGames.Pagamentos.Infrastructure.Data
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string Database { get; set; } = null!;

        public LoggingSettings Logging { get; set; } = new();
    }

    public class LoggingSettings
    {
        public string Database { get; set; } = null!;
        public string Collection { get; set; } = null!;
    }
}
