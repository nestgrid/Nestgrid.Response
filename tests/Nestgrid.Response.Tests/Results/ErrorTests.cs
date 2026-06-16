using Shouldly;

namespace Nestgrid.Response.Tests.Results;

public sealed class ErrorTests
{
    [Fact]
    public void Error_ShouldCreateErrorResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Error();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Error);
    }

    [Fact]
    public void Error_WithString_ShouldCreateErrorResultWithErrorMessage()
    {
        // Arrange
        const string message = "Unexpected failure";

        // Act
        var result = ResultsFactory.Error(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Error, message);
    }

    [Fact]
    public void Error_WithMessages_ShouldCreateErrorResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Unexpected failure");

        // Act
        var result = ResultsFactory.Error(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Error, message);
    }

    [Fact]
    public void Error_WithException_ShouldCreateErrorResultWithExceptionMessageAndCode()
    {
        // Arrange
        var exception = new InvalidOperationException("Unexpected failure");

        // Act
        var result = ResultsFactory.Error(exception);

        // Assert
        result.Status.ShouldBe(ResultStatus.Error);
        result.Messages.Count.ShouldBe(1);
        result.Messages[0].Message.ShouldBe(exception.Message);
        result.Messages[0].Code.ShouldBe(nameof(InvalidOperationException));
        result.Messages[0].Severity.ShouldBe(ResultMessageSeverity.Error);
    }

    [Fact]
    public void Error_WithNullException_ShouldThrowArgumentNullException()
    {
        // Arrange
        Exception exception = null!;

        // Act
        var thrown = Should.Throw<ArgumentNullException>(() => ResultsFactory.Error(exception));

        // Assert
        thrown.ParamName.ShouldBe(nameof(exception));
    }

    [Fact]
    public void ErrorOfT_ShouldCreateErrorResultWithDefaultValue()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Error<string>();

        // Assert
        FactoryAssertions.ShouldHaveDefaultValue(result, ResultStatus.Error);
    }

    [Fact]
    public void ErrorOfT_WithString_ShouldCreateErrorResultWithErrorMessage()
    {
        // Arrange
        const string message = "Unexpected failure";

        // Act
        var result = ResultsFactory.Error<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Error, message);
    }

    [Fact]
    public void ErrorOfT_WithMessages_ShouldCreateErrorResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Unexpected failure");

        // Act
        var result = ResultsFactory.Error<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Error, message);
    }

    [Fact]
    public void ErrorOfT_WithException_ShouldCreateErrorResultWithExceptionMessageAndCode()
    {
        // Arrange
        var exception = new InvalidOperationException("Unexpected failure");

        // Act
        var result = ResultsFactory.Error<string>(exception);

        // Assert
        result.Status.ShouldBe(ResultStatus.Error);
        result.Value.ShouldBeNull();
        result.Messages.Count.ShouldBe(1);
        result.Messages[0].Message.ShouldBe(exception.Message);
        result.Messages[0].Code.ShouldBe(nameof(InvalidOperationException));
        result.Messages[0].Severity.ShouldBe(ResultMessageSeverity.Error);
    }

    [Fact]
    public void ErrorOfT_WithNullException_ShouldThrowArgumentNullException()
    {
        // Arrange
        Exception exception = null!;

        // Act
        var thrown = Should.Throw<ArgumentNullException>(() => ResultsFactory.Error<string>(exception));

        // Assert
        thrown.ParamName.ShouldBe(nameof(exception));
    }
}
