using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;

namespace HireFlow.Infrustructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public ICompanyRepository Companies { get; }
    public IJobAdRepository JobAds { get; }
    public IRequestRepository Requests { get; }

    public UnitOfWork(
        AppDbContext context,
        ICompanyRepository companies,
        IJobAdRepository jobAds,
        IRequestRepository requests)
    {
        _context = context;
        Companies = companies;
        JobAds = jobAds;
        Requests = requests;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}