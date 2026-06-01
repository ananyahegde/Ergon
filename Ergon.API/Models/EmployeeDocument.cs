namespace Ergon.Models
{
    public enum DocumentTypeEnum
    {
        PanCard,
        AadhaarCard,
        PassportSizePhoto,
        Passbook,
        DegreeCertificate,
        PfNominationForm,
        RelievingLetter,
        ExperienceLetter,
        IncomeTaxDeclaration
    }

    public class EmployeeDocument
    {
        public Guid DocumentId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public DocumentTypeEnum DocumentType { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // foreign keys
        public Guid EmployeeId { get; set; }

        // navigation properties
        public Employee Employee { get; set; }

    }
}
