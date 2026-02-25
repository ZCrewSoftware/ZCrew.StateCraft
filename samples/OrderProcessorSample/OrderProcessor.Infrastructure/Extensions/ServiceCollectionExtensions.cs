using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessor.Domain.Contracts.Data;
using OrderProcessor.Infrastructure.Services;

namespace OrderProcessor.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderProcessorInfrastructure(this IServiceCollection services)
    {
        services.AddPooledDbContextFactory<OrderProcessorDbContext>(options =>
            options.UseInMemoryDatabase("OrderProcessorDb")
        );
        services.AddSingleton<IOrderDataService, OrderDataService>();
        return services;
    }
}
