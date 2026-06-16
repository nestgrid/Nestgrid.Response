namespace Nestgrid.Response.Tests.Results;

public sealed class NotFoundTests
{
    [Fact]
    public void NotFound_ShouldCreateNotFoundResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.NotFound();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.NotFound);
    }

    [Fact]
    public void NotFound_WithString_ShouldCreateNotFoundResultWithErrorMessage()
    {
        // Arrange
        const string message = "User not found";

        // Act
        var result = ResultsFactory.NotFound(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.NotFound, message);
    }

    [Fact]
    public void NotFound_WithMessages_ShouldCreateNotFoundResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("User not found");

        // Act
        var result = ResultsFactory.NotFound(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.NotFound, message);
    }

    [Fact]
    public void NotFoundOfT_ShouldCreateNotFoundResultWithDefaultValue()
    {
        // Arrange

        // Act
        var result = ResultsFactory.NotFound<string>();

        // Assert
        FactoryAssertions.ShouldHaveDefaultValue(result, ResultStatus.NotFound);
    }

    [Fact]
    public void NotFoundOfT_WithString_ShouldCreateNotFoundResultWithErrorMessage()
    {
        // Arrange
        const string message = "User not found";

        // Act
        var result = ResultsFactory.NotFound<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.NotFound, message);
    }

    [Fact]
    public void NotFoundOfT_WithMessages_ShouldCreateNotFoundResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("User not found");

        // Act
        var result = ResultsFactory.NotFound<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.NotFound, message);
    }
}
