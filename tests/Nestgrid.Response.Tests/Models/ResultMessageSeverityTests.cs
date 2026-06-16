using Shouldly;

namespace Nestgrid.Response.Tests.Models;

public sealed class ResultMessageSeverityTests
{
    [Fact]
    public void Values_ShouldMatchApprovedApi()
    {
        // Arrange
        var expected = new[]
        {
            ResultMessageSeverity.Information,
            ResultMessageSeverity.Warning,
            ResultMessageSeverity.Error
        };

        // Act
        var values = Enum.GetValues<ResultMessageSeverity>();

        // Assert
        values.ShouldBe(expected);
    }
}
