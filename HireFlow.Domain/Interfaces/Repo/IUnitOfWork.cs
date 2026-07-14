namespace HireFlow.Domain.Interfaces.Repo;

public interface IUnitOfWork
{
    ICompanyRepository Companies { get; }
    IJobAdRepository JobAds { get; }
    IRequestRepository Requests { get; }
    IProvinceRepository Provinces { get; }
    ICityRepository Cities { get; }
    ICategoryRepository Categories { get; }
    
    Task<int> SaveChangesAsync();
}