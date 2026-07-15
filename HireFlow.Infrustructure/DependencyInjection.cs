using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using HireFlow.Infrustructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HireFlow.Infrustructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
            
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IJobAdRepository, JobAdRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProvinceRepository, ProvinceRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        services.AddScoped<IJobAdRepository, JobAdRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        return services;
    }
}