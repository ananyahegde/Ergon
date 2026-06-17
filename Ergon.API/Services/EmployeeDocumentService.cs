using AutoMapper;
using Ergon.DTOs.EmployeeDocument;
using Ergon.Exceptions;
using Ergon.Interfaces;
using Ergon.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Ergon.Services
{
    public class EmployeeDocumentService : IEmployeeDocumentService
    {
        private readonly IRepository<Guid, EmployeeDocument> _repository;
        private readonly IRepository<Guid, Employee> _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeDocumentService(IRepository<Guid, EmployeeDocument> repository, IRepository<Guid, Employee> employeeRepository, IMapper mapper)
        {
            _repository = repository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDocumentResponse>> GetAllEmployeeDocumentsAsync(Guid employeeId)
        {
            var all = await _repository.GetAll();
            var documents = all.Where(d => d.EmployeeId == employeeId).ToList();
            return _mapper.Map<List<EmployeeDocumentResponse>>(documents);
        }

        public async Task<EmployeeDocumentResponse> GetEmployeeDocumentByIdAsync(Guid documentId)
        {
            var document = await _repository.Get(documentId);
            if (document == null) throw new NotFoundException("Document not found.");
            return _mapper.Map<EmployeeDocumentResponse>(document);
        }

        public async Task<EmployeeDocumentResponse> CreateEmployeeDocumentAsync(Guid employeeId, CreateEmployeeDocumentRequest request)
        {
            var employee = await _employeeRepository.Get(employeeId);
            if (employee == null) throw new NotFoundException("Employee not found.");

            if (request.DocumentType == DocumentTypeEnum.PassportSizePhoto)
            {
                var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedMimeTypes.Contains(request.File.ContentType.ToLower()))
                    throw new BadRequestException("Passport size photo must be JPG or PNG.");
            }
            else
            {
                if (request.File.ContentType.ToLower() != "application/pdf")
                    throw new BadRequestException("Document must be a PDF.");
            }

            if (request.File.Length > 5 * 1024 * 1024)
                throw new BadRequestException("File size must not exceed 5MB.");

            var ext = request.DocumentType == DocumentTypeEnum.PassportSizePhoto ? ".jpg" : ".pdf";
            var fileName = $"{employeeId}_{request.DocumentType}{ext}";
            var filePath = Path.Combine("uploads", "documents", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            if (request.DocumentType == DocumentTypeEnum.PassportSizePhoto)
            {
                using var image = await Image.LoadAsync(request.File.OpenReadStream());
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(512, 512),
                    Mode = ResizeMode.Max
                }));
                await image.SaveAsJpegAsync(filePath, new JpegEncoder { Quality = 80 });
            }
            else
            {
                using var inputStream = new MemoryStream();
                await request.File.CopyToAsync(inputStream);
                inputStream.Position = 0;

                var pdfDoc = PdfSharpCore.Pdf.IO.PdfReader.Open(inputStream, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Modify);
                pdfDoc.Options.CompressContentStreams = true;
                pdfDoc.Options.NoCompression = false;
                pdfDoc.Save(filePath);
            }

            var document = new EmployeeDocument
            {
                DocumentId = Guid.NewGuid(),
                DocumentName = request.DocumentName,
                DocumentType = request.DocumentType,
                FilePath = filePath,
                EmployeeId = employeeId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repository.Create(document);
            return _mapper.Map<EmployeeDocumentResponse>(document);
        }

        public async Task<EmployeeDocumentResponse> DeleteEmployeeDocumentAsync(Guid documentId)
        {
            var document = await _repository.Get(documentId);
            if (document == null) throw new NotFoundException("Document not found.");

            if (File.Exists(document.FilePath))
                File.Delete(document.FilePath);

            await _repository.Delete(documentId);
            return _mapper.Map<EmployeeDocumentResponse>(document);
        }

        public async Task<(byte[] fileBytes, string mimeType, string fileName)> DownloadEmployeeDocumentAsync(Guid documentId)
        {
            var document = await _repository.Get(documentId);
            if (document == null) throw new NotFoundException("Document not found.");
            if (!File.Exists(document.FilePath)) throw new NotFoundException("File not found on disk.");

            var fileName = Path.GetFileName(document.FilePath);
            var mimeType = document.FilePath.EndsWith(".pdf") ? "application/pdf" : "image/jpeg";
            var fileBytes = await File.ReadAllBytesAsync(document.FilePath);

            return (fileBytes, mimeType, fileName);
        }
    }
}
