using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;

public class CompanyRepository : GenericRepository<Company> , ICompanyRepository
{
    public CompanyRepository(AppDbContext context) : base(context)
    {
    }
}