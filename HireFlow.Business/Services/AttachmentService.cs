using HireFlow.Business.Exceptionss;
using HireFlow.Domain.Dtos.AttachmentDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using HireFlow.Domain.Interfaces.Repo;

namespace HireFlow.Business.Services;

public class AttachmentService : IAttachmentService 
{
    private readonly IUnitOfWork _unitOfWork;

    public AttachmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<AttachmentResultDto> UploadAsync(UploadAttachmentDto dto)
    {
        if (dto.Data == null || dto.Data.Length == 0)
            throw new InvalidFilePayloadException("File data cannot be empty.");

        
        var result = await _unitOfWork.Attachments.CreateAttachmentAsync(dto);

        
        await _unitOfWork.SaveChangesAsync();

        return result;
    }

    public async Task<AttachmentFileDto> DownloadAsync(Guid id)
    {
       
        if (id == Guid.Empty)
            throw new InvalidFilePayloadException("Attachment ID cannot be empty.");

       
        var fileDto = await _unitOfWork.Attachments.GetAttachmentFileByIdAsync(id);

        if (fileDto == null)
            throw new ItemNotFoundException("Attachment", id);

        return fileDto;
    }
}
