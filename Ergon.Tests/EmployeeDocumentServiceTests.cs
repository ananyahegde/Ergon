using AutoMapper;
using Ergon.DTOs.EmployeeDocument;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using Ergon.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Ergon.Tests
{
    public class EmployeeDocumentServiceTests
    {
        private Mock<IRepository<Guid, EmployeeDocument>> _mockRepo = null!;
        private Mock<IRepository<Guid, Employee>> _mockEmployeeRepo = null!;
        private Mock<IMapper> _mockMapper = null!;
        private EmployeeDocumentService _employeeDocumentService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<Guid, EmployeeDocument>>();
            _mockEmployeeRepo = new Mock<IRepository<Guid, Employee>>();
            _mockMapper = new Mock<IMapper>();
            _employeeDocumentService = new EmployeeDocumentService(_mockRepo.Object, _mockEmployeeRepo.Object, _mockMapper.Object);
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
            var docs = new List<EmployeeDocument>
            {
                new EmployeeDocument { DocumentId = Guid.NewGuid(), EmployeeId = employeeId, DocumentName = "Doc1", FilePath = "uploads/documents/doc1.pdf", DocumentType = DocumentTypeEnum.AadhaarCard },
                new EmployeeDocument { DocumentId = Guid.NewGuid(), EmployeeId = Guid.NewGuid(), DocumentName = "Doc2", FilePath = "uploads/documents/doc2.pdf", DocumentType = DocumentTypeEnum.AadhaarCard }
            };
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(docs);
            _mockMapper.Setup(m => m.Map<List<EmployeeDocumentResponse>>(It.IsAny<List<EmployeeDocument>>()))
                .Returns(new List<EmployeeDocumentResponse> { new() });

            var result = await _employeeDocumentService.GetAllEmployeeDocumentsAsync(employeeId);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetEmployeeDocumentById_DocumentExists_ReturnsResponse()
        {
            var documentId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Get(documentId))
                .ReturnsAsync(new EmployeeDocument { DocumentId = documentId, EmployeeId = Guid.NewGuid(), DocumentName = "Doc1", FilePath = "uploads/documents/doc1.pdf", DocumentType = DocumentTypeEnum.AadhaarCard });
            _mockMapper.Setup(m => m.Map<EmployeeDocumentResponse>(It.IsAny<EmployeeDocument>()))
                .Returns(new EmployeeDocumentResponse { DocumentId = documentId });

            var result = await _employeeDocumentService.GetEmployeeDocumentByIdAsync(documentId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DocumentId, Is.EqualTo(documentId));
        }

        [Test]
        public async Task GetEmployeeDocumentById_DocumentNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((EmployeeDocument?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeDocumentService.GetEmployeeDocumentByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task CreateEmployeeDocument_EmployeeNotFound_ThrowsNotFoundException()
        {
            _mockEmployeeRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Employee?)null);

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
            _mockEmployeeRepo.Setup(r => r.Get(employeeId)).ReturnsAsync(new Employee { EmployeeId = employeeId });

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
            _mockEmployeeRepo.Setup(r => r.Get(employeeId)).ReturnsAsync(new Employee { EmployeeId = employeeId });

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
            _mockEmployeeRepo.Setup(r => r.Get(employeeId)).ReturnsAsync(new Employee { EmployeeId = employeeId });

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
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((EmployeeDocument?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeDocumentService.DeleteEmployeeDocumentAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteEmployeeDocument_DocumentExists_RemovesFromDb()
        {
            var documentId = Guid.NewGuid();
            var doc = new EmployeeDocument { DocumentId = documentId, EmployeeId = Guid.NewGuid(), DocumentName = "Doc1", FilePath = "uploads/documents/doc1.pdf", DocumentType = DocumentTypeEnum.AadhaarCard };
            _mockRepo.Setup(r => r.Get(documentId)).ReturnsAsync(doc);
            _mockMapper.Setup(m => m.Map<EmployeeDocumentResponse>(It.IsAny<EmployeeDocument>()))
                .Returns(new EmployeeDocumentResponse { DocumentId = documentId });

            await _employeeDocumentService.DeleteEmployeeDocumentAsync(documentId);

            _mockRepo.Verify(r => r.Delete(documentId), Times.Once);
        }

        [Test]
        public async Task DownloadEmployeeDocument_DocumentNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((EmployeeDocument?)null);

            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeDocumentService.DownloadEmployeeDocumentAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DownloadEmployeeDocument_FileNotOnDisk_ThrowsNotFoundException()
        {
            var documentId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Get(documentId))
                .ReturnsAsync(new EmployeeDocument { DocumentId = documentId, EmployeeId = Guid.NewGuid(), DocumentName = "Doc1", FilePath = "uploads/documents/nonexistent.pdf", DocumentType = DocumentTypeEnum.AadhaarCard });

            Assert.ThrowsAsync<NotFoundException>(() =>
                _employeeDocumentService.DownloadEmployeeDocumentAsync(documentId));
        }
    }
}
