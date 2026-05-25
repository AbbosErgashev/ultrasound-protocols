using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UltrasoundProtocol.Domain.Interfaces;
using UltrasoundProtocol.Infrastructure.Data;
using UltrasoundProtocol.Infrastructure.Repositories;

namespace UltrasoundProtocol.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
