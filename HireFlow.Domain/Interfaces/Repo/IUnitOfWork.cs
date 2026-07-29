namespace HireFlow.Domain.Interfaces.Repo;

public interface IUnitOfWork
{
    ICompanyRepository Companies { get; }
    IJobAdRepository JobAds { get; }
    IRequestRepository Requests { get; }
    IProvinceRepository Provinces { get; }
    ICityRepository Cities { get; }
    ICategoryRepository Categories { get; }
    IAttachmentRepository Attachments { get; }
    ISkillRepository Skills { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IEmailTemplateRepository EmailTemplates { get; }
    Task<int> SaveChangesAsync();
}