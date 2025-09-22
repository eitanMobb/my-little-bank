namespace MyLittleBank.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AccountType { get; set; } = string.Empty; // "Checking" or "Savings"
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
