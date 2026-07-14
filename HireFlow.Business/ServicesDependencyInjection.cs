using HireFlow.Business.Authentications;
using HireFlow.Business.Services;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using Microsoft.Extensions.DependencyInjection;

namespace HireFlow.Business;

public static class ServicesDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IProvinceService, ProvinceService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<ICategoryService, CategoryService>();
        return services;
    }
}