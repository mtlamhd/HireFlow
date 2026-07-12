using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class JobAdRepository : GenericRepository<JobAd> , IJobAdRepository
{
    public JobAdRepository(AppDbContext context) : base(context)
    {
    }
}