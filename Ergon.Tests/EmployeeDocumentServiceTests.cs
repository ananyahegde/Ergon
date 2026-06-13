using AutoMapper;
using Ergon.Contexts;
using Ergon.DTOs.EmployeeDocument;
using Ergon.Exceptions;
using Ergon.Models;
using Ergon.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ergon.Tests
{
    public class EmployeeDocumentServiceTests
    {
        private ErgonContext _context = null!;
        private Mock<IMapper> _mockMapper = null!;
        private EmployeeDocumentService _employeeDocumentService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ErgonContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ErgonContext(options);
            _mockMapper = new Mock<IMapper>();
            _employeeDocumentService = new EmployeeDocumentService(_context, _mockMapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private IFormFile MakeMockFile(string contentType, long length)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.ContentType).Returns(contentType);
            mockFile.Setup(f => f.Length).Returns(length);
            mockFile.Setup(f => f.FileName).Returns("testfile");
            return mockFile.Object;
        }


        [Test]
        public async Task GetAllEmployeeDocuments_ReturnsOnlyEmployeeDocuments()
        {
            var employeeId = Guid.NewGuid();
            _context.EmployeeDocuments.AddRange(
                new EmployeeDocument { DocumentId = Guid.NewGuid(), EmployeeId = employeeId, DocumentName = "Doc1", FilePath = "uploads/documents/doc1.pdf", DocumentType = DocumentTypeEnum.AadhaarCard },
                new EmployeeDocument { DocumentId = Guid.NewGuid(), EmployeeId = Guid.NewGuid(), DocumentName = "Doc2", FilePath = "uploads/documents/doc2.pdf", DocumentType = DocumentTypeEnum.AadhaarCard }
            );
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<List<EmployeeDocumentResponse>>(It.IsAny<List<EmployeeDocument>>()))
                .Returns(new List<EmployeeDocumentResponse> { new() });

            var result = await _employeeDocumentService.GetAllEmployeeDocumentsAsync(employeeId);

            Assert.That(result.Count(), Is.EqualTo(1));
        }


        [Test]
        public async Task GetEmployeeDocumentById_DocumentExists_ReturnsResponse()
        {
            var documentId = Guid.NewGuid();
            _context.EmployeeDocuments.Add(new EmployeeDocument { DocumentId = documentId, EmployeeId = Guid.NewGuid(), DocumentName = "Doc1", FilePath = "uploads/documents/doc1.pdf", DocumentType = DocumentTypeEnum.AadhaarCard });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDocumentResponse>(It.IsAny<EmployeeDocument>()))
                .Returns(new EmployeeDocumentResponse { DocumentId = documentId });

            var result = await _employeeDocumentService.GetEmployeeDocumentByIdAsync(documentId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DocumentId, Is.EqualTo(documentId));
        }

        [Test]
        public async Task GetEmployeeDocumentById_DocumentNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeDocumentService.GetEmployeeDocumentByIdAsync(Guid.NewGuid()));
        }


        [Test]
        public async Task CreateEmployeeDocument_EmployeeNotFound_ThrowsNotFoundException()
        {
            var request = new CreateEmployeeDocumentRequest
            {
                DocumentName = "AadhaarCard",
                DocumentType = DocumentTypeEnum.AadhaarCard,
                File = MakeMockFile("application/pdf", 1024)
            };

            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeDocumentService.CreateEmployeeDocumentAsync(Guid.NewGuid(), request));
        }

        [Test]
        public async Task CreateEmployeeDocument_InvalidMimeForPassportPhoto_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            await _context.SaveChangesAsync();

            var request = new CreateEmployeeDocumentRequest
            {
                DocumentName = "Photo",
                DocumentType = DocumentTypeEnum.PassportSizePhoto,
                File = MakeMockFile("application/pdf", 1024)
            };

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeDocumentService.CreateEmployeeDocumentAsync(employeeId, request));
        }

        [Test]
        public async Task CreateEmployeeDocument_InvalidMimeForPdfDocument_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            await _context.SaveChangesAsync();

            var request = new CreateEmployeeDocumentRequest
            {
                DocumentName = "AadhaarCard",
                DocumentType = DocumentTypeEnum.AadhaarCard,
                File = MakeMockFile("image/jpeg", 1024)
            };

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeDocumentService.CreateEmployeeDocumentAsync(employeeId, request));
        }

        [Test]
        public async Task CreateEmployeeDocument_FileTooLarge_ThrowsBadRequestException()
        {
            var employeeId = Guid.NewGuid();
            _context.Employees.Add(new Employee { EmployeeId = employeeId, FirstName = "Arjun", LastName = "Nair" });
            await _context.SaveChangesAsync();

            var request = new CreateEmployeeDocumentRequest
            {
                DocumentName = "AadhaarCard",
                DocumentType = DocumentTypeEnum.AadhaarCard,
                File = MakeMockFile("application/pdf", 6 * 1024 * 1024)
            };

            Assert.ThrowsAsync<BadRequestException>(() =>
                _employeeDocumentService.CreateEmployeeDocumentAsync(employeeId, request));
        }


        [Test]
        public async Task DeleteEmployeeDocument_DocumentNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeDocumentService.DeleteEmployeeDocumentAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteEmployeeDocument_DocumentExists_RemovesFromDb()
        {
            var documentId = Guid.NewGuid();
            _context.EmployeeDocuments.Add(new EmployeeDocument { DocumentId = documentId, EmployeeId = Guid.NewGuid(), DocumentName = "Doc1", FilePath = "uploads/documents/doc1.pdf", DocumentType = DocumentTypeEnum.AadhaarCard });
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<EmployeeDocumentResponse>(It.IsAny<EmployeeDocument>()))
                .Returns(new EmployeeDocumentResponse { DocumentId = documentId });

            await _employeeDocumentService.DeleteEmployeeDocumentAsync(documentId);

            var deleted = await _context.EmployeeDocuments.FindAsync(documentId);
            Assert.That(deleted, Is.Null);
        }


        [Test]
        public async Task DownloadEmployeeDocument_DocumentNotFound_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeDocumentService.DownloadEmployeeDocumentAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DownloadEmployeeDocument_FileNotOnDisk_ThrowsNotFoundException()
        {
            var documentId = Guid.NewGuid();
            _context.EmployeeDocuments.Add(new EmployeeDocument { DocumentId = documentId, EmployeeId = Guid.NewGuid(), DocumentName = "Doc1", FilePath = "uploads/documents/nonexistent.pdf", DocumentType = DocumentTypeEnum.AadhaarCard });
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeDocumentService.DownloadEmployeeDocumentAsync(documentId));
        }
    }
}
