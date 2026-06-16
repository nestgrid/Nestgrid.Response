namespace Nestgrid.Response.Tests.Results;

public sealed class ConflictTests
{
    [Fact]
    public void Conflict_ShouldCreateConflictResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Conflict();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Conflict);
    }

    [Fact]
    public void Conflict_WithString_ShouldCreateConflictResultWithErrorMessage()
    {
        // Arrange
        const string message = "User already exists";

        // Act
        var result = ResultsFactory.Conflict(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Conflict, message);
    }

    [Fact]
    public void Conflict_WithMessages_ShouldCreateConflictResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("User already exists");

        // Act
        var result = ResultsFactory.Conflict(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Conflict, message);
    }

    [Fact]
    public void ConflictOfT_ShouldCreateConflictResultWithDefaultValue()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Conflict<string>();

        // Assert
        FactoryAssertions.ShouldHaveDefaultValue(result, ResultStatus.Conflict);
    }

    [Fact]
    public void ConflictOfT_WithString_ShouldCreateConflictResultWithErrorMessage()
    {
        // Arrange
        const string message = "User already exists";

        // Act
        var result = ResultsFactory.Conflict<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Conflict, message);
    }

    [Fact]
    public void ConflictOfT_WithMessages_ShouldCreateConflictResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("User already exists");

        // Act
        var result = ResultsFactory.Conflict<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Conflict, message);
    }
}
