using Nestgrid.Response.AspNetCore.Mappings;
using Nestgrid.Response.AspNetCore.Options;
using Shouldly;

namespace Nestgrid.Response.AspNetCore.Tests.Options;

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
