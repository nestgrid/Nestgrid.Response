namespace Nestgrid.Response.Tests.Results;

public sealed class NoContentTests
{
    [Fact]
    public void NoContent_ShouldCreateNoContentResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.NoContent();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.NoContent);
    }

    [Fact]
    public void NoContentOfT_ShouldCreateNoContentResultWithDefaultValue()
    {
        // Arrange

        // Act
        var result = ResultsFactory.NoContent<string>();

        // Assert
        FactoryAssertions.ShouldHaveDefaultValue(result, ResultStatus.NoContent);
    }
}
