using Microsoft.Extensions.DependencyInjection;
using OrderProcessor.Application.Context;
using OrderProcessor.Application.Context.Contracts;
using OrderProcessor.Application.Services;
using OrderProcessor.Domain.Contracts;

namespace OrderProcessor.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderProcessorApplication(this IServiceCollection services)
    {
        services.AddSingleton<IOrderContextStore, OrderContextStore>();
        services.AddScoped<IOrderCommandService, OrderCommandService>();
        services.AddScoped<ILineCommandService, LineCommandService>();
        return services;
    }
}
