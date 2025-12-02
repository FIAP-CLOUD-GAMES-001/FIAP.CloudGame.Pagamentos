namespace FIAP.CloudGames.Pagamentos.Domain.Requests
{
    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public decimal OrderAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
