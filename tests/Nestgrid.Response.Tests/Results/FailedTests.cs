namespace Nestgrid.Response.Tests.Results;

public sealed class FailedTests
{
    [Fact]
    public void Failed_ShouldCreateFailedResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Failed();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Failed);
    }

    [Fact]
    public void Failed_WithString_ShouldCreateFailedResultWithErrorMessage()
    {
        // Arrange
        const string message = "Operation failed";

        // Act
        var result = ResultsFactory.Failed(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Failed, message);
    }

    [Fact]
    public void Failed_WithMessages_ShouldCreateFailedResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Operation failed");

        // Act
        var result = ResultsFactory.Failed(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Failed, message);
    }

    [Fact]
    public void FailedOfT_ShouldCreateFailedResultWithDefaultValue()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Failed<string>();

        // Assert
        FactoryAssertions.ShouldHaveDefaultValue(result, ResultStatus.Failed);
    }

    [Fact]
    public void FailedOfT_WithString_ShouldCreateFailedResultWithErrorMessage()
    {
        // Arrange
        const string message = "Operation failed";

        // Act
        var result = ResultsFactory.Failed<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Failed, message);
    }

    [Fact]
    public void FailedOfT_WithMessages_ShouldCreateFailedResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Operation failed");

        // Act
        var result = ResultsFactory.Failed<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Failed, message);
    }
}
