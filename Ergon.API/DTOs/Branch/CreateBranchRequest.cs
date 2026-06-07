using System.ComponentModel.DataAnnotations;
namespace Ergon.DTOs.Branch
{
    public class CreateBranchRequest
    {
        [Required]
        [StringLength(100)]
        public string BranchName { get; set; } = string.Empty;
    }
}
