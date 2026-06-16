namespace Nestgrid.Response.Tests.Results;

public sealed class CancelledTests
{
    [Fact]
    public void Cancelled_ShouldCreateCancelledResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Cancelled();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Cancelled);
    }

    [Fact]
    public void Cancelled_WithString_ShouldCreateCancelledResultWithErrorMessage()
    {
        // Arrange
        const string message = "Operation cancelled";

        // Act
        var result = ResultsFactory.Cancelled(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Cancelled, message);
    }

    [Fact]
    public void Cancelled_WithMessages_ShouldCreateCancelledResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Operation cancelled");

        // Act
        var result = ResultsFactory.Cancelled(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Cancelled, message);
    }

    [Fact]
    public void CancelledOfT_ShouldCreateCancelledResultWithDefaultValue()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Cancelled<string>();

        // Assert
        FactoryAssertions.ShouldHaveDefaultValue(result, ResultStatus.Cancelled);
    }

    [Fact]
    public void CancelledOfT_WithString_ShouldCreateCancelledResultWithErrorMessage()
    {
        // Arrange
        const string message = "Operation cancelled";

        // Act
        var result = ResultsFactory.Cancelled<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Cancelled, message);
    }

    [Fact]
    public void CancelledOfT_WithMessages_ShouldCreateCancelledResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Operation cancelled");

        // Act
        var result = ResultsFactory.Cancelled<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Cancelled, message);
    }
}
