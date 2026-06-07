using System.ComponentModel.DataAnnotations;
namespace Ergon.DTOs.Branch
{
    public class UpdateBranchRequest
    {
        [Required]
        [StringLength(100)]
        public string BranchName { get; set; } = string.Empty;
    }
}
