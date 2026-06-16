namespace Nestgrid.Response.Tests.Results;

public sealed class ForbiddenTests
{
    [Fact]
    public void Forbidden_ShouldCreateForbiddenResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Forbidden();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Forbidden);
    }

    [Fact]
    public void Forbidden_WithString_ShouldCreateForbiddenResultWithErrorMessage()
    {
        // Arrange
        const string message = "Access denied";

        // Act
        var result = ResultsFactory.Forbidden(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Forbidden, message);
    }

    [Fact]
    public void Forbidden_WithMessages_ShouldCreateForbiddenResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Access denied");

        // Act
        var result = ResultsFactory.Forbidden(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Forbidden, message);
    }

    [Fact]
    public void ForbiddenOfT_ShouldCreateForbiddenResultWithDefaultValue()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Forbidden<string>();

        // Assert
        FactoryAssertions.ShouldHaveDefaultValue(result, ResultStatus.Forbidden);
    }

    [Fact]
    public void ForbiddenOfT_WithString_ShouldCreateForbiddenResultWithErrorMessage()
    {
        // Arrange
        const string message = "Access denied";

        // Act
        var result = ResultsFactory.Forbidden<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Forbidden, message);
    }

    [Fact]
    public void ForbiddenOfT_WithMessages_ShouldCreateForbiddenResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Access denied");

        // Act
        var result = ResultsFactory.Forbidden<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Forbidden, message);
    }
}
