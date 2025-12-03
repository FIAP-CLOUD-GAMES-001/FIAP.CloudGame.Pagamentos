namespace FIAP.CloudGames.Pagamentos.Domain.Responses
{
    public class PaymentResponse
    {
        public string Id { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime ProcessedDate { get; set; }
    }
}
