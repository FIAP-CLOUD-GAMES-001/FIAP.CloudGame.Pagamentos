namespace FIAP.CloudGames.Pagamentos.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Approved = 2,
        Rejected = 3
    }

    public enum PaymentMethod
    {
        CreditCard = 1,
        DebitCard,
        Pix,
        Boleto,
        GiftCard
    }
}
