using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class CompanyRepository : GenericRepository<Company> , ICompanyRepository
{
    public CompanyRepository(DbContext context) : base(context)
    {
    }
}