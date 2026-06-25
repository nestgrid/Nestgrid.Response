using Nestgrid.Response.Http.Mappings;
using Nestgrid.Response.Http.Options;
using Shouldly;

namespace Nestgrid.Response.Http.Tests.Options;

public sealed class NestgridResponseOptionsTests
{
    [Fact]
    public void Constructor_ShouldUseFullResultSuccessResponseMode()
    {
        // Act
        var options = new NestgridResponseOptions();

        // Assert
        options.SuccessResponseMode.ShouldBe(SuccessResponseMode.FullResult);
    }

    [Fact]
    public void Constructor_ShouldPopulateDefaultStatusMappings()
    {
        // Act
        var options = new NestgridResponseOptions();

        // Assert
        options.StatusMappings.ShouldBe(DefaultStatusMappings.Create());
    }
}
