using Nestgrid.Response.AspNetCore.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Nestgrid.Response.AspNetCore.Extensions;

/// <summary>
/// Provides Nestgrid.Response service registration.
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
        ArgumentNullException.ThrowIfNull(services);

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
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.Configure(configure);

        return services;
    }
}
