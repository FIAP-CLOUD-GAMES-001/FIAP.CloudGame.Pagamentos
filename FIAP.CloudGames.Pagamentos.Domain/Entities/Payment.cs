namespace FIAP.CloudGames.Pagamentos.Domain.Entities
{
    public class Payment
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public decimal OrderAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }

        public DateTime ProcessedDate { get; set; }
        public string PaymentStatus { get; set; }
    }
}
