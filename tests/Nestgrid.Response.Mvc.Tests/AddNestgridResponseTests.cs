using Nestgrid.Response.Http;
using Nestgrid.Response.Mvc.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nestgrid.Response.Http.Options;
using Shouldly;

namespace Nestgrid.Response.Mvc.Tests;

public sealed class AddNestgridResponseTests
{
    [Fact]
    public void AddNestgridResponse_WhenServicesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ServiceCollection services = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => services.AddNestgridResponse());

        // Assert
        exception.ParamName.ShouldBe(nameof(services));
        exception.StackTrace.ShouldStartWith(
            "   at Nestgrid.Response.Mvc.Extensions.ResponseServiceCollectionExtensions.AddNestgridResponse");
    }

    [Fact]
    public void AddNestgridResponse_ShouldRegisterOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddNestgridResponse();
        using var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetRequiredService<IOptions<NestgridResponseOptions>>().Value;
        options.SuccessResponseMode.ShouldBe(SuccessResponseMode.FullResult);
    }

    [Fact]
    public void AddNestgridResponse_WithConfigure_WhenServicesIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ServiceCollection services = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
            services.AddNestgridResponse(_ => { }));

        // Assert
        exception.ParamName.ShouldBe(nameof(services));
        exception.StackTrace.ShouldStartWith(
            "   at Nestgrid.Response.Mvc.Extensions.ResponseServiceCollectionExtensions.AddNestgridResponse");
    }

    [Fact]
    public void AddNestgridResponse_WithConfigure_WhenConfigureIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<NestgridResponseOptions> configure = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
            services.AddNestgridResponse(configure));

        // Assert
        exception.ParamName.ShouldBe(nameof(configure));
    }

    [Fact]
    public void AddNestgridResponse_WithConfigure_ShouldApplyConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddNestgridResponse(options =>
        {
            options.SuccessResponseMode = SuccessResponseMode.ValueOnly;
            options.StatusMappings[ResultStatus.Failed] = 400;
        });

        using var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetRequiredService<IOptions<NestgridResponseOptions>>().Value;
        options.SuccessResponseMode.ShouldBe(SuccessResponseMode.ValueOnly);
        options.StatusMappings[ResultStatus.Failed].ShouldBe(400);
    }
}
