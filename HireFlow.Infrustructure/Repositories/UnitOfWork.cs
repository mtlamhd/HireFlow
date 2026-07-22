using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;

namespace HireFlow.Infrustructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public ICompanyRepository Companies { get; }
    public IJobAdRepository JobAds { get; }
    public IRequestRepository Requests { get; }
    public IProvinceRepository Provinces { get; }
    public ICityRepository Cities { get; }
    public ICategoryRepository Categories { get; }
    public IAttachmentRepository Attachments { get; }
    public ISkillRepository Skills { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public UnitOfWork(
        AppDbContext context,
        ICompanyRepository companies,
        IJobAdRepository jobAds,
        IRequestRepository requests,
        IProvinceRepository provinces,
        ICityRepository cities,
        ICategoryRepository categories,
        IAttachmentRepository attachments, ISkillRepository skills, IRefreshTokenRepository refreshTokens)
    {
        _context = context;
        Companies = companies;
        JobAds = jobAds;
        Requests = requests;
        Provinces = provinces;
        Cities = cities;
        Categories = categories;
        Attachments = attachments;
        Skills = skills;
        RefreshTokens = refreshTokens;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}