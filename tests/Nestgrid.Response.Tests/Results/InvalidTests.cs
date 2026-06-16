namespace Nestgrid.Response.Tests.Results;

public sealed class InvalidTests
{
    [Fact]
    public void Invalid_ShouldCreateInvalidResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Invalid();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Invalid);
    }

    [Fact]
    public void Invalid_WithString_ShouldCreateInvalidResultWithErrorMessage()
    {
        // Arrange
        const string message = "Username is required";

        // Act
        var result = ResultsFactory.Invalid(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Invalid, message);
    }

    [Fact]
    public void Invalid_WithMessages_ShouldCreateInvalidResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Username is required");

        // Act
        var result = ResultsFactory.Invalid(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Invalid, message);
    }

    [Fact]
    public void InvalidOfT_ShouldCreateInvalidResultWithDefaultValue()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Invalid<string>();

        // Assert
        FactoryAssertions.ShouldHaveDefaultValue(result, ResultStatus.Invalid);
    }

    [Fact]
    public void InvalidOfT_WithString_ShouldCreateInvalidResultWithErrorMessage()
    {
        // Arrange
        const string message = "Username is required";

        // Act
        var result = ResultsFactory.Invalid<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Invalid, message);
    }

    [Fact]
    public void InvalidOfT_WithMessages_ShouldCreateInvalidResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Username is required");

        // Act
        var result = ResultsFactory.Invalid<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Invalid, message);
    }
}
