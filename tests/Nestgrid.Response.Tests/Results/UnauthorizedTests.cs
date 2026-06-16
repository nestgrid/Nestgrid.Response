namespace Nestgrid.Response.Tests.Results;

public sealed class UnauthorizedTests
{
    [Fact]
    public void Unauthorized_ShouldCreateUnauthorizedResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Unauthorized();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Unauthorized);
    }

    [Fact]
    public void Unauthorized_WithString_ShouldCreateUnauthorizedResultWithErrorMessage()
    {
        // Arrange
        const string message = "Authentication required";

        // Act
        var result = ResultsFactory.Unauthorized(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Unauthorized, message);
    }

    [Fact]
    public void Unauthorized_WithMessages_ShouldCreateUnauthorizedResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Authentication required");

        // Act
        var result = ResultsFactory.Unauthorized(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Unauthorized, message);
    }

    [Fact]
    public void UnauthorizedOfT_ShouldCreateUnauthorizedResultWithDefaultValue()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Unauthorized<string>();

        // Assert
        FactoryAssertions.ShouldHaveDefaultValue(result, ResultStatus.Unauthorized);
    }

    [Fact]
    public void UnauthorizedOfT_WithString_ShouldCreateUnauthorizedResultWithErrorMessage()
    {
        // Arrange
        const string message = "Authentication required";

        // Act
        var result = ResultsFactory.Unauthorized<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Unauthorized, message);
    }

    [Fact]
    public void UnauthorizedOfT_WithMessages_ShouldCreateUnauthorizedResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Authentication required");

        // Act
        var result = ResultsFactory.Unauthorized<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Unauthorized, message);
    }
}
