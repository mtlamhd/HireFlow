using HireFlow.Domain.Dtos.AttachmentDto;
using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public interface IAttachmentRepository : IGenericRepository<Attachment>
{
    Task<AttachmentResultDto> CreateAttachmentAsync(UploadAttachmentDto dto);
    
    Task<AttachmentFileDto?> GetAttachmentFileByIdAsync(Guid id);
}