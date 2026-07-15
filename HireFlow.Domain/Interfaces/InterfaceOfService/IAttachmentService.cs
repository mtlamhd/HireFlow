using HireFlow.Domain.Dtos.AttachmentDto;

namespace HireFlow.Domain.Interfaces.InterfaceOfService;

public interface IAttachmentService
{
    Task<AttachmentResultDto> UploadAsync(UploadAttachmentDto dto);

   
    Task<AttachmentFileDto> DownloadAsync(Guid id);
}