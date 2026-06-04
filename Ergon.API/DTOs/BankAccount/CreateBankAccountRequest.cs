namespace Ergon.DTOs.BankAccount
{
    public class BankAccountRequest
    {
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string IfscCode { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
