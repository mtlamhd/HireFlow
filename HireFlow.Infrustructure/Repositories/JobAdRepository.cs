using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class JobAdRepository : GenericRepository<JobAd> , IJobAdRepository
{
    public JobAdRepository(DbContext context) : base(context)
    {
    }
}