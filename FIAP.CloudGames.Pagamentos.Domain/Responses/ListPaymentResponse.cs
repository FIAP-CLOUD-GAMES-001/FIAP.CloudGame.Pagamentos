namespace FIAP.CloudGames.Pagamentos.Domain.Responses
{
    public class ListPaymentResponse
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public decimal OrderAmount { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ProcessedDate { get; set; }
    }
}
