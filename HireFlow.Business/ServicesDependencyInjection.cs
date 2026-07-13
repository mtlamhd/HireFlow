using HireFlow.Business.Authentications;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using Microsoft.Extensions.DependencyInjection;

namespace HireFlow.Business;

public static class ServicesDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        return services;
    }
}