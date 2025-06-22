namespace SwiftApplicationAPI.Models.CommandDtos
{
    public class LastTransactionDTO
    {
        public string TransactionId { get; set; }
        public string YourName { get; set; }
        public string ReceiverName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
