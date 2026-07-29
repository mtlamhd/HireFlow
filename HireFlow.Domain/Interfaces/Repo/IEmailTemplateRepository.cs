using HireFlow.Domain.Dtos.EmailDto;
using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repo;

public interface IEmailTemplateRepository : IGenericRepository<EmailTemplate>
{
    Task<List<EmailTemplateDto>> GetAllTemplatesAsync();

   
    Task<EmailTemplateDto?> GetTemplateByIdAsync(Guid id);

   
    Task<EmailTemplate?> GetTemplateEntityByIdAsync(Guid id);
}