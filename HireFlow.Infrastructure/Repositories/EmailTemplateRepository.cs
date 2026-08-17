using HireFlow.Domain.Dtos.EmailDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Repositories;

public class EmailTemplateRepository : GenericRepository<EmailTemplate>, IEmailTemplateRepository
{
    public EmailTemplateRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<EmailTemplateDto>> GetAllTemplatesAsync()
    {
       
        return await _dbSet
            .AsNoTracking()
            .OrderBy(t => t.Type)
            .Select(t => new EmailTemplateDto
            {
                Id = t.Id,
                Type = t.Type,
                Subject = t.Subject,
                Body = t.Body,
                IsActive = t.IsActive
            })
            .ToListAsync();
    }

    public async Task<EmailTemplateDto?> GetTemplateByIdAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new EmailTemplateDto
            {
                Id = t.Id,
                Type = t.Type,
                Subject = t.Subject,
                Body = t.Body,
                IsActive = t.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EmailTemplate?> GetTemplateEntityByIdAsync(Guid id)
    {
       
        return await GetByIdAsync(id, tracking: true);
    }
}