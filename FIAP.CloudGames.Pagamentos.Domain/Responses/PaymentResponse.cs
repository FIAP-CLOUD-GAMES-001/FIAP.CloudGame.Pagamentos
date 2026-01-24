using FIAP.CloudGames.Pagamentos.Domain.Enums;

namespace FIAP.CloudGames.Pagamentos.Domain.Responses;

public class PaymentResponse
{
    public string Id { get; set; } = string.Empty;
    public EPaymentStatus PaymentStatus { get; set; }
    public DateTime ProcessedDate { get; set; }
}