using System.ComponentModel.DataAnnotations;

namespace Ergon.DTOs.BankAccount
{
    public class BankAccountRequest
    {
        [Required(ErrorMessage = "Bank name is required.")]
        public string BankName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account number is required.")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "IFSC code is required.")]
        public string IfscCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account holder name is required.")]
        public string AccountHolderName { get; set; } = string.Empty;
    }
}
