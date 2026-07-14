using HireFlow.Domain.Dtos.AttachmentDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class AttachmentRepository : GenericRepository<Attachment>, IAttachmentRepository
{
    public AttachmentRepository(AppDbContext context) : base(context)
    {
    }
    public async Task<AttachmentResultDto> CreateAttachmentAsync(UploadAttachmentDto dto)
    {
        
        var attachment = new Attachment(
            fileName: dto.FileName,
            contentType: dto.ContentType,
            filePath: null,
            fileSize: dto.Data.Length,
            data: dto.Data
        );

        await _dbSet.AddAsync(attachment);

       
        return new AttachmentResultDto
        {
            Id = attachment.Id,
            FileName = attachment.FileName
        };
    }

    public async Task<AttachmentFileDto?> GetAttachmentFileByIdAsync(Guid id)
    {
        
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AttachmentFileDto
            {
                Data = a.Data,
                ContentType = a.ContentType,
                FileName = a.FileName
            })
            .FirstOrDefaultAsync();
    }
}
