using Microsoft.Extensions.DependencyInjection;
using Nestgrid.Response.Http.Options;

namespace Nestgrid.Response.Mvc.Extensions;

/// <summary>
/// Provides Nestgrid.Response service registration for MVC adapters.
/// </summary>
public static class ResponseServiceCollectionExtensions
{
    /// <summary>
    /// Registers Nestgrid.Response options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddNestgridResponse(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddOptions<NestgridResponseOptions>();

        return services;
    }

    /// <summary>
    /// Registers and configures Nestgrid.Response options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The options configuration delegate.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddNestgridResponse(
        this IServiceCollection services,
        Action<NestgridResponseOptions> configure)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        services.Configure(configure);

        return services;
    }
}
