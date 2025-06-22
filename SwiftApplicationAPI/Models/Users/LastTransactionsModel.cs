using System.ComponentModel.DataAnnotations;

namespace SwiftApplicationAPI.Models.Users
{
    public class LastTransactionsModel
    {
        public string TransactionId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public decimal RecieverAmount { get; set; }
        public string RecieverCurrency { get; set; }
        public decimal OriginalAmount { get; set; }
        public string OriginalCurrency { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}