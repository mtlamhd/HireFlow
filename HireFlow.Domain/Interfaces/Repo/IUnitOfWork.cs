namespace HireFlow.Domain.Interfaces.Repo;

public interface IUnitOfWork
{
    ICompanyRepository Companies { get; }
    IJobAdRepository JobAds { get; }
    IRequestRepository Requests { get; }
    
    Task<int> SaveChangesAsync();
}