using FIAP.CloudGames.Pagamentos.Domain.Enums;

namespace FIAP.CloudGames.Pagamentos.Domain.Requests;

public class PaymentRequest
{
    public string OrderId { get; set; } = string.Empty;
    public decimal OrderAmount { get; set; }
    public EPaymentMethod PaymentMethod { get; set; }
    public DateTime OrderDate { get; set; }
}