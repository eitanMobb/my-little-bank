namespace MyLittleBank.Models
{
    public class TransferRequest
    {
        public string FromAccount { get; set; } = string.Empty;
        public string ToAccount { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
