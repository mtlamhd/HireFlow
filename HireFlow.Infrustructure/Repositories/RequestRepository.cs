using Azure.Core;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;
using Request = HireFlow.Domain.Entities.Request;

namespace HireFlow.Infrustructure.Repositories;

public class RequestRepository : GenericRepository<Request>, IRequestRepository
{
    public RequestRepository(AppDbContext context) : base(context)
    {
    }
}