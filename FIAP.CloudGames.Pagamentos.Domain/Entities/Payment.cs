using FIAP.CloudGames.Pagamentos.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FIAP.CloudGames.Pagamentos.Domain.Entities
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private set; }

        public string PaymentId { get; private set; }      // Gerado internamente
        public string OrderId { get; private set; }        // Vem da API de pedidos
        public decimal OrderAmount { get; private set; }
        public EPaymentMethod PaymentMethod { get; private set; }
        public DateTime OrderDate { get; private set; }

        public DateTime? ProcessedDate { get; private set; }
        public EPaymentStatus PaymentStatus { get; private set; }

        public Payment(
            string orderId,
            decimal orderAmount,
            EPaymentMethod paymentMethod,
            DateTime orderDate
        )
        {
            Id = ObjectId.GenerateNewId().ToString();           // Mongo _id
            PaymentId = Guid.NewGuid().ToString("N");           // ID do negócio
            OrderId = orderId;
            OrderAmount = orderAmount;
            PaymentMethod = paymentMethod;
            OrderDate = orderDate;

            PaymentStatus = EPaymentStatus.Pending;
        }

        public void Approve()
        {
            PaymentStatus = EPaymentStatus.Approved;
            ProcessedDate = DateTime.UtcNow;
        }

        public void Reject()
        {
            PaymentStatus = EPaymentStatus.Rejected;
            ProcessedDate = DateTime.UtcNow;
        }
    }

    
}
